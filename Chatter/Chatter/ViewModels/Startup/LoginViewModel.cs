using Chatter.Entities;
using Chatter.Models.Startup;
using Chatter.Repositories;
using Chatter.Services;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    private readonly UserDataRepository _userDataRepository;
    public IAsyncRelayCommand<Image> RunAnimationCommand { get; }

    public User User { get; set; }
    public IAsyncRelayCommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }
    public bool IsLoading { get; set; }
    public Image LogoImage { get; set; }

    public LoginViewModel(IApiService apiService, IMessageCollector messageCollector, UserDataRepository userDataRepository)
    {
        _apiService = apiService;
        _userDataRepository = userDataRepository;
        _messageCollector = messageCollector;
        User = new User() {
            Username = "kubspl",
            Password = "zaqzaq"
        };

        LoginCommand = new AsyncRelayCommand(Login);
        RegisterCommand = new Command(Register);
        RunAnimationCommand = new AsyncRelayCommand<Image>(async (LogoImage) => {
            
            var a1 = LogoImage!.RotateTo(180, 1000);
            var a2 = LogoImage!.FadeTo(0, 1000);

            await Task.WhenAll(a1, a2);

            var a3 = LogoImage!.RotateTo(0, 1000);
            var a4 = LogoImage!.FadeTo(1, 1000);

            await Task.WhenAll(a3, a4);
        });
    }

    private async Task Login()
    {
        IsLoading = true;

        bool success = await _apiService.LoginUserAsync(User);
        IsLoading = false;
        if (success) {
            string rsaKey = TryCreateRSA();
            await _apiService.PostPublicKeyAsync(rsaKey);
            _messageCollector.StartCollectingMessages();
            var toast = Toast.Make("Starting service...", ToastDuration.Long, 15);
            await toast.Show();

            await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
        } else {
            // huh, unlucky
        }
    }

    private string TryCreateRSA()
    {
        _userDataRepository.UpdateConnection();
        if (_userDataRepository.UserDataExists(User.Username)) {
            return CryptographyService.GetPublicKey(_userDataRepository.GetCurrentUserData()!.XmlKeys);
        }

        var rsaCreds = CryptographyService.GenerateNewKeyPair();

        _userDataRepository.AddUserData(rsaCreds.ToXmlString(true));

        var toast = Toast.Make("RSA keys created!", ToastDuration.Long, 15);
        toast.Show();

        return rsaCreds.ToXmlString(false);
    }

    private async void Register()
    {
        await Shell.Current.Navigation.PushAsync(RegisterView.Create());
    }

}
