using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PigBot.Database;

namespace PigBot.Etc
{
    public class FourchanWatcher : IFourchanWatcher
    {
        private PigbotDbContext dbContext;
        private string threadUrl;
        private DateTime lastUpdate = DateTime.Now;
        
        public FourchanWatcher(PigbotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task UpdateFourchan()
        {
            var threadUrl = await GetCurrentThread();
            var client = new HttpClient();
            var response = await client.GetAsync(threadUrl);
            var responseText = await response.Content.ReadAsStringAsync();
            var thread = JsonConvert.DeserializeObject<Thread>(responseText);
            
            foreach (var post in thread.Posts)
            {
                var dbPost = dbContext.FourchanPosts.FirstOrDefault(p => p.PostId == post.No);
                if (dbPost == null)
                {
                    var fourchanPost = new FourchanPost();
                    fourchanPost.Content = post.Com;
                    fourchanPost.DateTime = post.Now;
                    fourchanPost.FileUrl = post.Tim + post.Ext;
                    fourchanPost.PostId = post.No;

                    dbContext.Add(fourchanPost);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task<string> GetCurrentThread()
        {
            if (threadUrl != null && DateTime.Now.Subtract(TimeSpan.FromMinutes(30)) < lastUpdate)
            {
                Console.WriteLine("no need to update thread");
                return threadUrl;
            }
            
            Console.WriteLine("updating thread");
            var client = new HttpClient();
            var response = await client.GetAsync("http://chinkshit.xyz");
            var responseUri = response.RequestMessage.RequestUri.ToString();
            
            threadUrl = ExtractApiUrl(responseUri);
            lastUpdate = DateTime.Now;
            
            return await GetCurrentThread();
        }

        private static string ExtractApiUrl(string threadUrl)
        {
            var threadParts = threadUrl.Split("/");
            var threadId = "";
            
            foreach (var threadPart in threadParts)
            {
                if (int.TryParse(threadPart, out var i))
                {
                    threadId = threadPart;
                }
            }

            return $"https://a.4cdn.org/g/thread/{threadId}.json";
        }
    }
}