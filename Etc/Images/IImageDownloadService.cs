namespace PigBot.Etc.Images
{
    public interface IImageDownloadService
    {
        string DownloadImage(string imageUrl);
        void DeleteImage(string fileName);
    }
}