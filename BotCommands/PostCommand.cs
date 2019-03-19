using System;
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
        private readonly DiscordSocketClient discordClient;

        public PostCommand(PigbotDbContext dbContext, IImageDownloadService imageDownloadService, DiscordSocketClient discordClient)
        {
            this.dbContext = dbContext;
            this.imageDownloadService = imageDownloadService;
            this.discordClient = discordClient;
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
                var channel = discordClient
                    .GetGuild(189466684938125312)
                    .GetTextChannel(491172628917256192);

                if (channel == null)
                {
                    Console.WriteLine("channel is null");
                }
                    
                await channel.SendFileAsync(fileStream, fileName, "Post", false, embedBuilder.Build());
                
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