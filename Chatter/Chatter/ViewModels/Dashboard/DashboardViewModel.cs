using Chatter.Models.Dashboard;
using Chatter.Services;
using Chatter.Views.Dashboard;
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

    public ObservableCollection<User> Friends { get; } = new();
    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand InviteCommand { get; }
    public bool IsRefreshing { get; set; }
    public DashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
        RefreshCommand = new Command(RefreshAsync);
        SearchCommand = new Command(GoToSearchView);
        InviteCommand = new Command(GoToInviteView);
        IsRefreshing = false;
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
        //var users = await _apiService.GetFriendsAsync();
        IsRefreshing = true;
        var users = new[] {
            new User{
                Username = "kubspl"
            },
            new User{
                Username = "kubs2"
            },
            new User{
                Username = "kubatuba"
            },
            new User {
                Username = "maciek"
            },
            new User {
                Username = "monia",
                IsOnline = true
            }
        };

        Friends.Clear();

        foreach (var user in users) {
            Friends.Add(user);
        }

        IsRefreshing = false;
    }
}
