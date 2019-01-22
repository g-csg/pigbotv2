using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PigBot.Etc.WolframAlpha;

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
            return message.Content.StartsWith("!!answer_test");
        }

        public async Task Execute(SocketMessage message)
        {
            var requestQuery = message.Content.Replace("!!answer_test", "");

            message.Channel.TriggerTypingAsync();
            var result = wolframAlphaClient.ApiCall(requestQuery);
            if (result == null)
            {
                await message.Channel.SendMessageAsync("No answer was found to this question");
                return;
            }

            var relevantPods = result.queryresult.pods.Take(2);
            
            foreach (var pod in relevantPods)
            {
                if (pod.title == "Input interpretation")
                {
                    continue;
                }
                
                var embedBuilder = new EmbedBuilder();
                embedBuilder.Fields.Add(new EmbedFieldBuilder().WithName(pod.title).WithValue("Wolfram|Alpha Response"));
                embedBuilder.WithImageUrl(pod.subpods.First(s => s.img != null).img.src);

                Console.WriteLine("embed builder done");
                try
                {
                    var embedMessage = embedBuilder.Build();
                    await message.Channel.SendMessageAsync(embed: embedMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException?.Message);
                }
                
            }
        }
    }
}