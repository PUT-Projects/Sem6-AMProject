using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Chatter.Platforms.Android;

namespace Chatter
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MainActivity : MauiAppCompatActivity
    {
        public MainActivity()
        {
            AndroidServiceManager.MainActivity = this;
        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            ProcessIntent(intent);
        }
        public void StartService()
        {
            var serviceIntent = new Intent(this, typeof(BackgroundService));
            StartService(serviceIntent);
        }

        public void StopService()
        {
            var serviceIntent = new Intent(this, typeof(BackgroundService));
            StopService(serviceIntent);
        }
        private void ProcessIntent(Intent? intent)
        {
            if (intent is null) return;

            var action = intent.Action;
            if (action == "USER_TAPPED_NOTIFIACTION") {
               
                // show ChatView
                var username = intent.GetStringExtra("username");
                // var chatView = Views.ChatView.Create(username!);
                
                // idk how to display the chatView so it will just go to dashboard xd
                // Shell.Current.Navigation.PushAsync(chatView);
            }

        }
    }
}
