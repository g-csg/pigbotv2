using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PigBot.Etc.WolframAlpha.DTO;

namespace PigBot.Etc.WolframAlpha
{
    public class WolframAlphaClient : IWolframAlphaClient
    {
        private readonly IConfiguration configuration;

        public WolframAlphaClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ApiResponse ApiCall(string query)
        {
            query = Uri.EscapeDataString(query);
            
            var callUrl =
                $"https://api.wolframalpha.com/v2/query?input={query}&appid={configuration["WolframAlphaKey"]}&output=json";

            var webClient = new HttpClient();
            var response = webClient.GetAsync(callUrl).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                return apiResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: ");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException?.Message);
            }
            return new ApiResponse();
        }
    }
}