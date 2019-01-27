using System;
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
                    try
                    {
                        await botCommand.Execute(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine($"Exception executing command: {botCommand.GetType().FullName}");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.InnerException?.Message);
                    }
                }
            }
        }
    }
}