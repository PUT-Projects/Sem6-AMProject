using Chatter.Models.Startup;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels.Startup;

public sealed class LoginPageViewModel : ViewModelBase
{
    private readonly RegisterPage registerPage;

    public User User { get; set; }
    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }

    public LoginPageViewModel(RegisterPage registerPage)
    {
        User = new User();
        LoginCommand = new Command(Login);
        RegisterCommand = new Command(Register);
        this.registerPage = registerPage;
    }

    private async void Login()
    {
        await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
    }

    private async void Register()
    {
        await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
    }
}
