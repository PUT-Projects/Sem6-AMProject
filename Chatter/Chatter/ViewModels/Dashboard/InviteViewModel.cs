using Chatter.Models.Dashboard;
using Chatter.Services;
using Chatter.Views.Dashboard;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

// import the vibrator for Android
#if __ANDROID__
using Android.OS;
#endif

namespace Chatter.ViewModels.Dashboard;

public class InviteViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private string _searchQuery = string.Empty;
    public ICommand BackCommand { get; }
    public IAsyncRelayCommand InviteCommand { get; }
    public ObservableCollection<SearchUser> Users { get; } = new();
    public string SearchQuery { get => _searchQuery; set => Search(value); }

    public InviteViewModel(IApiService apiService)
    {
        _apiService = apiService;
        BackCommand = new Command(GoBack);
        InviteCommand = new AsyncRelayCommand<string>(Invite);
    }

    public void OnAppearing(SearchBar searchBar)
    {
        searchBar.Focus();
    }

    private async Task Invite(string? username)
    {
#if __ANDROID__ 
        var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Android.Content.Context.VibratorService)!;
        vibrator!.Vibrate(VibrationEffect.CreateOneShot(100, VibrationEffect.DefaultAmplitude));
#endif
        var toast = Toast.Make("Invitation sent to " + username!);
        await toast.Show();
        await _apiService.InviteFriendAsync(username!);
    }

    private async void Search(string newValue)
    {
        if (_searchQuery == newValue) return;

        _searchQuery = newValue;

        if (string.IsNullOrWhiteSpace(_searchQuery)) {
            Users.Clear();
            return;
        }

        var users = await _apiService.SearchUsersAsync(_searchQuery);

        Users.Clear();
        foreach (var user in users) {
            Users.Add(user);
        }
    }

    private async void GoBack()
    {
        await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
    }
}
