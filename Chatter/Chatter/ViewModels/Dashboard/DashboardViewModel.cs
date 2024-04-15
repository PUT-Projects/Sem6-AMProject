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
    public INavigation Navigation { get; set; }
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
        var chatView = CreateChatView(username);

        if (chatView == null) {
            var toast = Toast.Make("ChatView not found");
            await toast.Show();
            return;
        }

        await Navigation.PushAsync(chatView);
    }

    private async void GoToSearchView()
    { 
        await Shell.Current.GoToAsync($"//{nameof(SearchView)}");
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

    private ChatView? CreateChatView(string username)
    {
        var chatView = Application.Current!.MainPage!
            .Handler!.MauiContext!.Services.GetService<ChatView>();

        if (chatView is null) return null;

        var vm = (ChatViewModel)chatView.BindingContext;
        vm.User = new User { Username = username };

        return chatView;
    }
}
