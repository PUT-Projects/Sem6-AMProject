using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.App;
using Chatter.Models;
using Chatter.Models.Dashboard;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Binder = Android.OS.Binder;

namespace Chatter.Platforms.Android;

[Service]
public class BackgroundService : Service
{
    private int id = (new object()).GetHashCode();
    private int badgeCount = 0;
    private Timer? timer = null;
    private NotificationCompat.Builder? builder = null;
    private readonly IBinder binder = new LocalBinder();

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        builder = new NotificationCompat.Builder(this, MainApplication.ChannelId)
            .SetContentTitle("Chatter")
            .SetContentText("Chatter is running!")
            .SetSmallIcon(Resource.Drawable.message)
            .SetAutoCancel(false)
            .SetPriority(NotificationCompat.PriorityDefault);

        var notification = builder.Build();

        StartForeground(id, notification);

        timer = new Timer(Timer_Elapsed, builder, 0, 1000);

        AndroidServiceManager.MessageCollector!.AddObserver(OnNewMessages);

        AndroidServiceManager.IsRunning = true;

        return StartCommandResult.Sticky;
    }

    private async void Timer_Elapsed(object? state)
    {
        if (!AndroidServiceManager.IsRunning) {
            ForceStop();
            return;
        }

        try {
            await AndroidServiceManager.MessageCollector!.CollectAndNotify();
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private void ForceStop()
    {
        AndroidServiceManager.MessageCollector?.RemoveObserver(OnNewMessages);

        timer?.Change(Timeout.Infinite, Timeout.Infinite);
        timer?.Dispose();
        timer = null;

        StopSelf();
    }

    private async Task OnNewMessages(IEnumerable<GetMessageDto> messages)
    {
        // select usernames from messages
        var usernames = messages.Select(m => m.Sender).Distinct();

        foreach (var username in usernames) {
            if (!IsMuted(username)) {
                ShowNotification(username);
            }
        }
    }

    private bool IsMuted(string username)
    {
        return username == AndroidServiceManager.MutedUser;
    }

    private void ShowNotification(string username)
    {
        var user = new User() {
            Username = username
        };

        var notification = CreateNotification(user);

        if (notification is null) return;

        int id = username.GetDeterministicHashCode();

        var notificationManager = NotificationManagerCompat.From(this);
        notificationManager.Notify(id, notification);
    }

    private Notification? CreateNotification(User user)
    {
        var notificationIntent = new Intent(this, typeof(MainActivity));
        notificationIntent.SetAction("USER_TAPPED_NOTIFIACTION");
        notificationIntent.PutExtra("username", user.Username);

        var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent,
                       PendingIntentFlags.Immutable);

        builder?
            .SetContentTitle(user.Username)
            .SetContentText("New message!")
            .SetLargeIcon(GetIcon(user.ImageUrl))
            .SetContentIntent(pendingIntent);

        return builder?.Build();
    }

    private Bitmap? GetIcon(string imageUrl)
    {
        int begin = "ProfilePictures/".Length;
        string resourceName = imageUrl[begin..^4];

        var type = typeof(Resource.Drawable);
        var field = type.GetField(resourceName, BindingFlags.Public | BindingFlags.Static)!;
        var resourceId = (int)field.GetValue(null)!;

        return BitmapFactory.DecodeResource(Resources, resourceId);
    }


    #region Service Implementation
    public class LocalBinder : Binder
    {
        public BackgroundService GetService()
        {
            return this.GetService();
        }
    }

    public override IBinder OnBind(Intent? intent)
    {
        return binder;
    }
#endregion
}
