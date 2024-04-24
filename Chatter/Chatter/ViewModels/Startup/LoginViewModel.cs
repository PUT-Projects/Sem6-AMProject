using Chatter.Models.Startup;
using Chatter.Services;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if ANDROID
using Chatter.Platforms.Android;
#endif

namespace Chatter.ViewModels.Startup;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private readonly IMessageCollector _messageCollector;

    public User User { get; set; }
    public IAsyncRelayCommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }
    public bool IsLoading { get; set; }

    public LoginViewModel(IApiService apiService, IMessageCollector messageCollector)
    {
        _apiService = apiService;
        _messageCollector = messageCollector;
        User = new User() {
            Username = "kubspl",
            Password = "zaqzaq"
        };

        LoginCommand = new AsyncRelayCommand(Login);
        RegisterCommand = new Command(Register);
    }

    private async Task Login()
    {
        IsLoading = true;
        bool success = await _apiService.LoginUserAsync(User);
        IsLoading = false;
        if (success) {
            _messageCollector.StartCollectingMessages();
            var toast = Toast.Make("Starting service...", ToastDuration.Long, 15);
            await toast.Show();

            await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
        } else {
            // huh, unlucky
        }
    }

    private async void Register()
    {
        await Shell.Current.Navigation.PushAsync(RegisterView.Create());
    }
}
