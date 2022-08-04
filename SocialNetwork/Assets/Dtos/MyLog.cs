using Serilog;

namespace SocialNetwork.Assets.Dtos
{
    public class MyLog
    {
        public static void E(string message)
        {
            Log.Error(""+message);
        }
    }
}
