using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PigBot.BotCommands
{
	public class ReplaceGBReflinkCommand: IBotCommand
	{
		public bool CanExecute(SocketMessage message)
		{
			// TODO: mobile gearbest links
			return message.Content.Contains("https://gearbest.com");
		}

		public async Task Execute(SocketMessage message)
		{
			var replaceMessage = message.Content;

			var aliParser = new Regex(@"\b(?:https://s\.gearbest\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			foreach (Match match in aliParser.Matches(message.Content))
			{
				try
				{
					var httpClient = new HttpClient();
					var result = await httpClient.GetAsync(match.Value);
					var responseUrl = result.RequestMessage.RequestUri.ToString().Split("?").First();
					replaceMessage = replaceMessage.Replace(match.Value, responseUrl);
				}
				catch (Exception e)
				{
					Console.WriteLine("Could not replace: "+e.Message);
				}
			}

			var aliMobileParser = new Regex(@"\b(?:https://gearbest\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			foreach (Match match in aliMobileParser.Matches(message.Content))
			{
				Console.WriteLine(match.Value);
				var cleanUrl = match.Value.Split("?").First();
				replaceMessage = replaceMessage.Replace(match.Value, cleanUrl);
			}

			var embedBuilder = new EmbedBuilder();
			embedBuilder.Color = Color.Magenta;

			embedBuilder.AddField(new EmbedFieldBuilder().WithName("Attention")
					.WithValue("The message contained an Gearbest referral link. Here is the replaced version:"));
			embedBuilder.AddField(new EmbedFieldBuilder().WithName($"Cleaned message from {message.Author.Username}")
					.WithValue(replaceMessage));

			await message.DeleteAsync();
			await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
		}
	}
}
