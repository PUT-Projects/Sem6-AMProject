using Chatter.Models.Startup;
using Chatter.Services;
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

public sealed class LoginViewModel : ViewModelBase
{
    private readonly IApiService _apiService;

    public User User { get; set; }
    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }
    public bool IsLoading { get; set; }

    public LoginViewModel(IApiService apiService)
    {
        User = new User() {
            Username = "kubspl",
            Password = "zaqzaq"
        };

        LoginCommand = new Command(Login);
        RegisterCommand = new Command(Register);
        _apiService = apiService;
    }

    private async void Login()
    {
        IsLoading = true;
        bool success = await _apiService.LoginUserAsync(User);
        IsLoading = false;
        if (success) {
            await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
        } else {

        }
    }

    private async void Register()
    {
        await Shell.Current.GoToAsync($"//{nameof(RegisterView)}");
    }
}
