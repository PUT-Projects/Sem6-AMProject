using Chatter.Models;
using Chatter.ViewModels.Dashboard;
using Chatter.ViewModels.Startup;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Chatter;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        ConfigureServices(builder.Services);
        
        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ViewModels
        services.AddSingleton<LoginPageViewModel>();
        services.AddSingleton<RegisterPageViewModel>();
        services.AddSingleton<DashboardPageViewModel>();


        // Views
        services.AddSingleton<LoginPage>();
        services.AddSingleton<RegisterPage>();
        services.AddSingleton<DashboardPage>();
    }
}
