using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PigBot.Database;
using PigBot.Etc.Images;

namespace PigBot.BotCommands
{
    public class PostCommand: IBotCommand
    {
        private readonly PigbotDbContext dbContext;
        private readonly IImageDownloadService imageDownloadService;

        public PostCommand(PigbotDbContext dbContext, IImageDownloadService imageDownloadService)
        {
            this.dbContext = dbContext;
            this.imageDownloadService = imageDownloadService;
        }

        public bool CanExecute(SocketMessage message)
        {
            var authorId = message.Author.Id;
            var adminUser = dbContext.AdminUsers.FirstOrDefault(a => a.UserId == authorId);
            return message.Content.StartsWith("!post") && adminUser != null;
        }

        public async Task Execute(SocketMessage message)
        {
            var parts = message.Content.Split(" ");
            if (int.TryParse(parts.Last(), out var postId))
            {
                var post = dbContext.FourchanPosts.FirstOrDefault(p => p.PostId == postId);
                if (post == null)
                {
                    await message.Channel.SendMessageAsync("Post not found");
                    return;
                }
                
                var embedBuilder = new EmbedBuilder();
                embedBuilder.Color = Color.Green;
                embedBuilder.Fields.Add(new EmbedFieldBuilder().WithName("Post").WithValue(
                    WebUtility.HtmlDecode(Regex.Replace(post.Content.Replace("<br>", "\n"), "<[^>]*(>|$)", string.Empty))));
                
                var fileName = "";
                
                if (!string.IsNullOrEmpty(post.FileUrl) && post.FileUrl != "0")
                {
                    var downloadUrl = $"https://i.4cdn.org/g/{post.FileUrl}";
                    fileName = imageDownloadService.DownloadImage(downloadUrl);
                    embedBuilder.WithImageUrl($"attachment://{fileName}");
                }

                var fileStream = new FileStream(fileName, FileMode.Open);
                var result = await message.Channel.SendFileAsync(
                    filename: fileName, 
                    stream: fileStream, 
                    embed: embedBuilder.Build());
                
                fileStream.Dispose();
                
                if (!string.IsNullOrEmpty(fileName))
                {
                    imageDownloadService.DeleteImage(fileName);
                }

                return;
            }

            await message.Channel.SendMessageAsync("that's not a :b:ost");
        }
    }
}