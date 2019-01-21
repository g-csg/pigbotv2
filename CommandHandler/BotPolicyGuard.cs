using System;
using System.Linq;
using Discord.WebSocket;
using PigBot.BotCommands;
using PigBot.Database;

namespace PigBot.CommandHandler
{
    public class BotPolicyGuard : IBotPolicyGuard
    {
        private readonly PigbotDbContext pigBotDbContext;

        public BotPolicyGuard(PigbotDbContext pigBotDbContext)
        {
            this.pigBotDbContext = pigBotDbContext;
        }

        public bool ExecutionAllowed(IBotCommand botCommand, SocketMessage message)
        {
            var commandName = botCommand.GetType().FullName;
            var userId = message.Author.Id;
            var guild = message.Channel as SocketGuildChannel;
            var guildId = guild?.Guild.Id;
            var dateTime = DateTime.Now;

            
            Console.WriteLine("command policy");
            Console.WriteLine(commandName);
            if (pigBotDbContext.BlackLists.FirstOrDefault(b => b.UserId == userId) != null)
            {
                return false;
            }

            if (guild == null)
            {
                return true;
            }
            
            if (pigBotDbContext.CommandPolicies.FirstOrDefault(c => c.Allowed == false && c.GuildId == guildId && c.CommandName == commandName) != null)
            {
                return false;
            }

            var cooldownPolicy =
                pigBotDbContext.CoolDownPolicies.FirstOrDefault(c =>
                    c.CommandName == commandName && c.GuildId == guildId);

            if (cooldownPolicy != null)
            {
                if (cooldownPolicy.LastExecution.AddSeconds(cooldownPolicy.CooldownSeconds) >= dateTime)
                {
                    return false;
                }
            }

            return true;
        }
    }
}