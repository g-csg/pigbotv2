using Discord.WebSocket;
using PigBot.BotCommands;

namespace PigBot.CommandHandler
{
    public interface IBotPolicyGuard
    {
        bool ExecutionAllowed(IBotCommand botCommand, SocketMessage message);
    }
}