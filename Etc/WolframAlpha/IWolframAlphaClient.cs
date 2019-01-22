using PigBot.Etc.WolframAlpha.DTO;

namespace PigBot.Etc.WolframAlpha
{
    public interface IWolframAlphaClient
    {
        ApiResponse ApiCall(string query);
    }
}