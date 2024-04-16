using Chatter.Services;
using Chatter.ViewModels;
using Chatter.ViewModels.Startup;
using Chatter.Views.Startup;
using Microsoft.Maui.ApplicationModel;

namespace Chatter;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        var vm = serviceProvider.GetRequiredService<AppShellViewModel>();
        MainPage = new AppShell(vm);
    }
}
