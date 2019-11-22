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

        // ???
        public bool CanExecute(SocketMessage message)
        {
            var authorId = message.Author.Id;
            var adminUser = dbContext.AdminUsers.FirstOrDefault(a => a.UserId == authorId);
            // adminUser seems to be related to some database idea that wasn't implemented
            // see Database/Adminuser.cs
            return message.Content.StartsWith("!post") == true;
        }


        // Post the Fourchan post as a Discord Embed message
        public async Task Execute(SocketMessage message)
        {
            // grab post from Fourchan
            var parts = message.Content.Split(" ");
            if (int.TryParse(parts.Last(), out var postId))
            {
                var post = dbContext.FourchanPosts.FirstOrDefault(p => p.PostId == postId);
                if (post == null)
                {
                    await message.Channel.SendMessageAsync("Post not found");
                    return;
                }

                // setup text channel to post in 
                var channel = discordClient
                    .GetGuild(189466684938125312)	 // the csg Discord server
                    .GetTextChannel(491172628917256192); // the channel #csg-reviews

                if (channel == null)
                {
                    await message.Channel.SendMessageAsync("Text channel not found! Where do I post?");
                    return;
                }

                // Build embed message
                var embedBuilder = new EmbedBuilder();
                embedBuilder.Color = Color.Green;
                embedBuilder.Fields.Add(new EmbedFieldBuilder().WithName($"{postId}").WithValue(
                          WebUtility.HtmlDecode(Regex.Replace(post.Content.Replace("<br>", "\n"), "<[^>]*(>|$)", string.Empty))));

                // if there is an image with the Fourchan post, include it
                if (!string.IsNullOrEmpty(post.FileUrl) && post.FileUrl != "0")
                {
                    var downloadUrl = $"https://i.4cdn.org/g/{post.FileUrl}";
                    var fileName = imageDownloadService.DownloadImage(downloadUrl);
                    var fileStream = new FileStream(fileName, FileMode.Open);
                    embedBuilder.WithImageUrl($"attachment://{fileName}");
                    
                    // post message with picture
                    await channel.SendFileAsync(fileStream, fileName, "", false, embedBuilder.Build());

                    // clean up
                    fileStream.Dispose();
                    imageDownloadService.DeleteImage(fileName);
                }
                // if there is no image, post the text
                else
                {
                    await channel.SendMessageAsync("", false, embedBuilder.Build());
                }

                return;
            }

            await message.Channel.SendMessageAsync("that's not a post!");

        }
    }
}
