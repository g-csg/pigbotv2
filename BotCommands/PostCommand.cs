using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using PigBot.Database;

namespace PigBot.BotCommands
{
    public class PostCommand: IBotCommand
    {
        private PigbotDbContext dbContext;

        public PostCommand(PigbotDbContext dbContext)
        {
            this.dbContext = dbContext;
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
            }

            await message.Channel.SendMessageAsync("that's not a :b:ost");
        }
    }
}