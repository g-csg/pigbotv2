using System.Threading.Tasks;
using Discord.WebSocket;

namespace PigBot.CommandHandler
{
    public interface IBotCommandHandler
    {
        Task OnMessage(SocketMessage message);
    }
}