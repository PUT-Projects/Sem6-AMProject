using Chatter.Services;
using Chatter.ViewModels;
using Chatter.Views;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;

namespace Chatter;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
