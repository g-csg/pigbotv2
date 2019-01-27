namespace PigBot.Etc.WolframAlpha
{
    public interface IWolframAlphaClient
    {
        string ApiCall(string query);
    }
}