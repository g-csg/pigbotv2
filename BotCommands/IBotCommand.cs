using System.Threading.Tasks;
using Discord.WebSocket;

namespace PigBot.BotCommands
{
    public interface IBotCommand
    {
        bool CanExecute(SocketMessage message);
        Task Execute(SocketMessage message);
    }
}