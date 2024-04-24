using Android.Content;
using Chatter;

namespace Chatter.Platforms.Android;
public static class AndroidServiceManager
{
    public static MainActivity? MainActivity { get; set; }
    public static string Username { get; set; }
    public static bool IsRunning { get; set; }
    public static AndroidMessageCollector? MessageCollector { get; set; }
    public static string MutedUser { get; set; }

    public static void Start()
    {
        MainActivity?.StartService();
    }

    public static void Stop()
    {
        MainActivity?.StopService();
        IsRunning = false;
    }
}