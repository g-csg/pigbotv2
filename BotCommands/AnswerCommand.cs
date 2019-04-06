using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;
using PigBot.Etc;
using PigBot.Etc.WolframAlpha;
using Thread = System.Threading.Thread;

namespace PigBot.BotCommands
{
    public class AnswerCommand: IBotCommand
    {
        private readonly IWolframAlphaClient wolframAlphaClient;

        public AnswerCommand(IWolframAlphaClient wolframAlphaClient)
        {
            this.wolframAlphaClient = wolframAlphaClient;
        }

        public bool CanExecute(SocketMessage message)
        {
            return message.Content.StartsWith("!answer");
        }

        public async Task Execute(SocketMessage message)
        {
            var requestQuery = message.Content.Replace("!answer ", "");

            if (requestQuery == "what is the heaviest object in the universe")
            {
                var matthewResponse = new EmbedBuilder();
                matthewResponse.AddField(new EmbedFieldBuilder().WithName("Result").WithValue("Matthew"));
                await message.Channel.SendMessageAsync(embed: matthewResponse.Build());
                return;
            }
            
            await message.Channel.TriggerTypingAsync();
            var result = wolframAlphaClient.ApiCall(requestQuery);
            if (result == null)
            {
                await message.Channel.SendMessageAsync("Wolfram|Alpha seems to be down");
                return;
            }

            if (result.Contains("No short answer available") || result.Contains("Wolfram|Alpha did not understand your input"))
            {
                var errorBuilder = new EmbedBuilder();
                errorBuilder.AddField(new EmbedFieldBuilder().WithName("Error")
                    .WithValue("Your question could not be answered"));
                errorBuilder.Color = Color.Red;
                
                var answer = await message.Channel.SendMessageAsync(embed: errorBuilder.Build());

                new Thread(() => 
                { 
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    message.DeleteAsync();
                    answer.DeleteAsync();
                }).Start();
                return;
            }
            
            var embedBuilder = new EmbedBuilder();
            embedBuilder.AddField(new EmbedFieldBuilder().WithName("Result").WithValue(result));

            await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}