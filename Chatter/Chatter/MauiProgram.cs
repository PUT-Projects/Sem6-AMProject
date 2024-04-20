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
using Chatter.Repositories;
using Chatter.ViewModels;
using Chatter.Views;
#if ANDROID
using Android.Content.Res;
#endif

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

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) => {
#if ANDROID
            handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            handler.PlatformView.SetPadding(5, 0, 5, 0);
#elif __IOS__
        handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
        handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None; 
#endif
        });

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        var settings = new Settings();
        configuration.GetSection("Settings").Bind(settings);
        services.AddSingleton(settings);
        // bind settings
        services.AddHttpClient();
        services.AddTransient<MessageRepository>();

        services.AddSingleton<MessageCollectorService>();
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<DbSettings>();

        // ViewModels
        services.AddSingleton<AppShellViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<RegisterViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddSingleton<InviteViewModel>();
        services.AddSingleton<AcceptViewModel>();
        services.AddTransient<ChatViewModel>();


        // Views
        services.AddSingleton<LoginView>();
        services.AddSingleton<RegisterView>();
        services.AddSingleton<DashboardView>();
        services.AddTransient<SearchView>();
        services.AddSingleton<InviteView>();
        services.AddSingleton<AcceptView>();
        services.AddTransient<ChatView>();

    }
}
