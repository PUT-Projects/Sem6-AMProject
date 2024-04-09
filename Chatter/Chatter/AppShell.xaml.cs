using Chatter.Services;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;

namespace Chatter;

public partial class AppShell : Shell
{
    private readonly IApiService _apiService;

    public AppShell(IApiService apiService)
    {
        _apiService = apiService;
        InitializeComponent();

        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(DashboardView), typeof(DashboardView));
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        _apiService.Logout();
        Shell.Current.FlyoutIsPresented = false;
        Current.GoToAsync($"//{nameof(LoginView)}");
    }
}
