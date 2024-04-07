using Chatter.ViewModels.Startup;
using Chatter.Views.Startup;
using Microsoft.Maui.ApplicationModel;

namespace Chatter;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
