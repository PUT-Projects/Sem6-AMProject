﻿using Chatter.Entities;
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

    public User User { get; set; }
    public IAsyncRelayCommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }
    public bool IsLoading { get; set; }

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
    }

    private async Task Login()
    {
        IsLoading = true;
        bool success = await _apiService.LoginUserAsync(User);
        IsLoading = false;
        if (success) {
            TryCreateRSA();
            _messageCollector.StartCollectingMessages();
            var toast = Toast.Make("Starting service...", ToastDuration.Long, 15);
            await toast.Show();

            await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
        } else {
            // huh, unlucky
        }
    }

    private void TryCreateRSA()
    {
        _userDataRepository.UpdateConnection();
        if (_userDataRepository.UserDataExists(User.Username)) return;

        string rsaCreds = CryptographyService.GenerateNewKeyPairXml();

        _userDataRepository.AddUserData(rsaCreds);

        var toast = Toast.Make("RSA keys created!", ToastDuration.Long, 15);
        toast.Show();
    }

    private async void Register()
    {
        await Shell.Current.Navigation.PushAsync(RegisterView.Create());
    }
}
