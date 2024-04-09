using Chatter.Models.Dashboard;
using Chatter.Services;
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
    public DashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    private async Task RefreshAsync()
    {
        //var users = await _apiService.GetFriendsAsync();

        var users = new[] {
            new User{
                Username = "kubspl"
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
    }
}
