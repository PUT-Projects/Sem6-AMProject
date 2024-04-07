using Chatter.Models;
using Chatter.ViewModels.Dashboard;
using Chatter.ViewModels.Startup;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Runtime;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Chatter.Services;

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
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Chatter.appsettings.json")) {
            builder.Configuration.AddJsonStream(stream!);
        }

        ConfigureServices(builder.Services, builder.Configuration);
        
        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        var settings = new Settings();
        configuration.GetSection("Settings").Bind(settings);
        services.AddSingleton(settings);
        // bind settings

        services.AddSingleton<IApiService, ApiService>();

        services.AddHttpClient();
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
