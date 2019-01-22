using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace PigBot.Etc.Images
{
    public class ImageDownloadService : IImageDownloadService
    {
        public string DownloadImage(string imageUrl)
        {
            var fileName = Guid.NewGuid() + "." + imageUrl.Split(".").Last();
            var httpClient = new HttpClient();
            var contentStream = httpClient.GetStreamAsync(imageUrl).GetAwaiter().GetResult();
            var fileStream = new FileStream(fileName, FileMode.Create);
            contentStream.CopyTo(fileStream);
            return fileName;
        }

        public void DeleteImage(string fileName)
        {
            File.Delete(fileName);
        }
    }
}