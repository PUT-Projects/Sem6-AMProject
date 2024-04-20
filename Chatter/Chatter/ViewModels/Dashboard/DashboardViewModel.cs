using Chatter.Models.Dashboard;
using Chatter.Services;
using Chatter.Views;
using Chatter.Views.Dashboard;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels.Dashboard;

public sealed class DashboardViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    public ObservableCollection<User> Friends { get; } = [];
    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand InviteCommand { get; }
    public ICommand UserSelectedCommand { get; }
    public bool IsRefreshing { get; set; }
    public DashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
        RefreshCommand = new Command(RefreshAsync);
        SearchCommand = new Command(GoToSearchView);
        InviteCommand = new Command(GoToInviteView);
        UserSelectedCommand = new Command<string>(UserSelected);
        IsRefreshing = false;
    }
    private async void UserSelected(string username)
    {
        var chatView = ChatView.Create(username);

        if (chatView == null) {
            var toast = Toast.Make("ChatView not found");
            await toast.Show();
            return;
        }

        await Shell.Current.Navigation.PushAsync(chatView);
    }

    private async void GoToSearchView()
    { 
        var searchView = SearchView.Create();

        if (searchView == null) {
            var toast = Toast.Make("SearchView not found");
            await toast.Show();
            return;
        }

        await Shell.Current.Navigation.PushAsync(searchView);
    }

    private async void GoToInviteView()
    {
        await Shell.Current.GoToAsync($"//{nameof(InviteView)}");
    }

    private async void RefreshAsync()
    {
        var users = await _apiService.GetFriendsAsync();

        Friends.Clear();

        foreach (var user in users) {
            Friends.Add(user);
        }

        IsRefreshing = false;
    }
}
