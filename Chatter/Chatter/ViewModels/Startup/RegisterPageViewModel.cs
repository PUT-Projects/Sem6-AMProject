using Chatter.Models.Startup;
using Chatter.Services;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels.Startup;

public sealed class RegisterPageViewModel : ViewModelBase
{
    private readonly IApiService _apiService;

    public RegisterUser User { get; set; }
    public ICommand RegisterCommand { get; }
    public ICommand BackCommand { get; }
    public bool IsLoading { get; set; }
    public RegisterPageViewModel(IApiService apiService)
    {
        _apiService = apiService;

        User = new RegisterUser() {
            Username = "kubspl",
            Password = "zaqzaq",
            ConfirmPassword = "zaqzaq"
        };
        RegisterCommand = new Command(Register);
        BackCommand = new Command(OnBackButtonPressed);
        IsLoading = false;
    }

    public void UpdateColors(Frame entryFrame, Frame entryIconFrame, Image entryIcon)
    {
        var color = PasswordsMatch() ? Colors.Green : Colors.Red;
        UpdateBorderColors(color, entryFrame, entryIconFrame, entryIcon);
    }

    private void UpdateBorderColors(Color color, Frame entryFrame, Frame entryIconFrame, Image entryIcon)
    {
        entryFrame.BorderColor = color;
        entryIconFrame.BorderColor = color;
        entryIcon.Behaviors.Clear();
        entryIcon.Behaviors.Add(new IconTintColorBehavior { TintColor = color });
    }

    private async void Register()
    {
        TrimInput();

        if (!await AnalyzeInputAndHighlightErrors()) return;

        IsLoading = true;
        if (!await _apiService.RegisterUserAsync(User)) {
            // registration failed
            IsLoading = false;
            return;
        }

        await _apiService.LoginUserAsync(User);
        ClearUser();
        IsLoading = false;

        await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
    }

    private async void OnBackButtonPressed()
    {
        ClearUser();

        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private async Task<bool> AnalyzeInputAndHighlightErrors()
    {
        if (string.IsNullOrWhiteSpace(User.Username) || string.IsNullOrWhiteSpace(User.Password) || string.IsNullOrWhiteSpace(User.ConfirmPassword)) {
            var toast = Toast.Make("Please fill in all fields.", ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        if (!PasswordsMatch()) {
            var toast = Toast.Make("Passwords do not match.", ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        if (User.Password.Length < 6) {
            var toast = Toast.Make("Password must be at least 8 characters long.", ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        return true;
    }
    private bool PasswordsMatch()
    {
        return User.Password == User.ConfirmPassword;
    }

    private void ClearUser()
    {
        User.Username = string.Empty;
        User.Password = string.Empty;
        User.ConfirmPassword = string.Empty;
    }

    private void TrimInput()
    {
        User.Username = User.Username.Trim();
        User.Password = User.Password.Trim();
        User.ConfirmPassword = User.ConfirmPassword.Trim();
    }
}
