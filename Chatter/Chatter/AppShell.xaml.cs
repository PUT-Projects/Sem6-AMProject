using Chatter.Services;
using Chatter.ViewModels;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;

namespace Chatter;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(DashboardView), typeof(DashboardView));
        Routing.RegisterRoute(nameof(SearchView), typeof(SearchView));
        Routing.RegisterRoute(nameof(InviteView), typeof(InviteView));
        Routing.RegisterRoute(nameof(AcceptView), typeof(AcceptView));
    }

}
