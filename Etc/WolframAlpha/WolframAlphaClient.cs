using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace PigBot.Etc.WolframAlpha
{
    public class WolframAlphaClient : IWolframAlphaClient
    {
        private readonly IConfiguration configuration;

        public WolframAlphaClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ApiCall(string query)
        {
            query = Uri.EscapeDataString(query);
            
            var callUrl =
                $"https://api.wolframalpha.com/v1/result?i={query}&appid={configuration["WolframAlphaKey"]}&units=metric";

            var webClient = new HttpClient();
            var response = webClient.GetAsync(callUrl).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return content;
        }
    }
}