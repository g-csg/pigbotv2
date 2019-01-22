using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PigBot.BotCommands;
using PigBot.CommandHandler;
using PigBot.Etc;
using Thread = System.Threading.Thread;

namespace PigBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var container = Container.CreateContainer();
            var client = new DiscordSocketClient();

            var commandHandler = container.GetService<IBotCommandHandler>();
            
            client.MessageReceived += commandHandler.OnMessage;
            client.MessageReceived += ClientOnMessageReceived;
            client.Connected += ClientOnConnected;
            
            await client.LoginAsync(TokenType.Bot, container.GetService<IConfiguration>()["DiscordApiKey"]);
            await client.StartAsync();

            var fourchanWatcher = container.GetService<IFourchanWatcher>();

            new Thread(() =>
            {
                fourchanWatcher.UpdateFourchan();
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }).Start();
            
            // Prevent bot from exiting
            await Task.Delay(-1);
        }

        private static async Task ClientOnMessageReceived(SocketMessage message)
        {
            if (!message.Content.StartsWith("!commands"))
            {
                return;
            }

            var container = Container.CreateContainer();
            var botCommands = container.GetServices<IBotCommand>();

            var messageBuilder = new StringBuilder();
            foreach (var botCommand in botCommands)
            {
                messageBuilder.Append(botCommand.GetType().FullName + Environment.NewLine);
            }

            await message.Channel.SendMessageAsync(messageBuilder.ToString());
        }

        private static async Task ClientOnConnected()
        {
            Console.WriteLine("connected");
        }
    }
}
