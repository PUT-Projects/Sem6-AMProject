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

public sealed class DashboardPageViewModel : ViewModelBase
{
    private readonly IApiService _apiService;

    public ObservableCollection<User> Friends { get; } = new();
    public ICommand RefreshCommand { get; }
    public DashboardPageViewModel(IApiService apiService)
    {
        _apiService = apiService;
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    private async Task RefreshAsync()
    {
        var users = await _apiService.GetFriendsAsync();

        Friends.Clear();

        foreach (var user in users) {
            Friends.Add(user);
        }
    }
}
