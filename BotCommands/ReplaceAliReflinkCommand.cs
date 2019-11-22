// ReplaceAliReflinkCommand.cs - Detects and replaces Aliexpress referrall links
// Currently supports desktop "s.click"-type referral links, with and without HTTP(S)
// There are two types of "s.click"-type referall links: Mobile and Desktop
// Mobile s.click links go through one more round of 'cleaning'
// there are also desktop referral links that don't use s.click.... shideded

using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PigBot.BotCommands
{
	public class ReplaceAliReflinkCommand: IBotCommand
	{
		public bool CanExecute(SocketMessage message)
		{
			// check for *s.click* in user message
			Regex CheckSClick = new Regex(@"(?:.*//s\.click)\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			return CheckSClick.IsMatch(message.Content);
		}

		public async Task Execute(SocketMessage message)
		{
			//await message.Channel.SendMessageAsync("A s.click referral link was detected!"); // debug
			var replaceMessage = message.Content;

			var aliParser = new Regex(@"(?:.*//s\.click)\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			foreach (Match match in aliParser.Matches(message.Content))
			{
				try
				{
					var httpClient = new HttpClient();
					var result = await httpClient.GetAsync(match.Value);
					// await message.Channel.SendMessageAsync(result.RequestMessage.ToString()); // debug
					// if redirectUrl is found in the 'expanded' referral link, extract the actual product url
					// there has to be a less potato way to do this... TODO: clean me up ;)
					if(result.RequestMessage.ToString().Contains("redirectUrl"))
					{
						// csgraduate.jpg
						var responseUrl = result.RequestMessage.RequestUri.ToString().Split("redirectUrl=").Last().Split("m%252F").Last().Split("%252F").First();
						// this is definitely wrong, but it works(tm)
						responseUrl = "https://www.aliexpress.com/item/" + responseUrl + ".html";
						replaceMessage = replaceMessage.Replace(match.Value, responseUrl);
					}
					else
					{
						var responseUrl = result.RequestMessage.RequestUri.ToString().Split("?").First();
						replaceMessage = replaceMessage.Replace(match.Value, responseUrl);
					}

				}
				catch (Exception e)
				{
					Console.WriteLine("Could not replace: "+e.Message);
				}
			}

			var aliMobileParser = new Regex(@"\b(?:https://m\.aliexpress\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			foreach (Match match in aliMobileParser.Matches(message.Content))
			{
				Console.WriteLine(match.Value);
				var cleanUrl = match.Value.Split("?").First();
				replaceMessage = replaceMessage.Replace(match.Value, cleanUrl);
			}

			var embedBuilder = new EmbedBuilder();
			embedBuilder.Color = Color.Magenta;

			embedBuilder.AddField(new EmbedFieldBuilder().WithName("Attention")
					.WithValue("The message contained an Ali Express ref link. Here is the replaced version:"));
			embedBuilder.AddField(new EmbedFieldBuilder().WithName($"Cleaned message from {message.Author.Username}")
					.WithValue(replaceMessage));

			await message.DeleteAsync();
			await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
		}
	}
}
