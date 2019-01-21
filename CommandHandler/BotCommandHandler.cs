using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using PigBot.BotCommands;

namespace PigBot.CommandHandler
{
    public class BotCommandHandler : IBotCommandHandler
    {
        private readonly IEnumerable<IBotCommand> botCommands;
        private readonly IBotPolicyGuard botPolicyGuard;
        
        public BotCommandHandler(IEnumerable<IBotCommand> botCommands, IBotPolicyGuard botPolicyGuard)
        {
            this.botCommands = botCommands;
            this.botPolicyGuard = botPolicyGuard;
        }

        public async Task OnMessage(SocketMessage message)
        {
            foreach (var botCommand in botCommands)
            {
                if (botCommand.CanExecute(message) && botPolicyGuard.ExecutionAllowed(botCommand, message))
                {
                    await botCommand.Execute(message);
                }
            }
        }
    }
}