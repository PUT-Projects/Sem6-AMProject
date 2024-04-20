using Chatter.Models.Dashboard;
using Chatter.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels.Dashboard;

public class AcceptViewModel : ViewModelBase
{
    private bool _backgroundServiceRunning = false;
    private readonly IApiService _apiService;
    public ObservableCollection<AcceptUser> FriendRequests { get; } = new();
    public ICommand AcceptCommand { get; }
    public ICommand RejectCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public bool IsRefreshing { get; set; }
    public AcceptViewModel(IApiService apiService)
    {
        _apiService = apiService;

        AcceptCommand = new Command<string>(Accept);
        RejectCommand = new Command<string>(Reject);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        IsRefreshing = false;
        
        FriendRequests.Add(new AcceptUser { Username = "JohnDoe" });
        FriendRequests.Add(new AcceptUser { Username = "JaneDoe" });
        FriendRequests.Add(new AcceptUser { Username = "JohnSmith" });
        FriendRequests.Add(new AcceptUser { Username = "Alice" });
    }

    public void RunApiService()
    {
        if (_backgroundServiceRunning) return;
        _backgroundServiceRunning = true;
        Task.Factory.StartNew(RefreshLoop, TaskCreationOptions.LongRunning);
    }

    private async void RefreshLoop()
    {
        while (true) {
            await Task.Delay(60000);
            await RefreshAsync();
        }
    }
    
    private async Task RefreshAsync()
    {
        var requests = await _apiService.GetFriendRequests();

        FriendRequests.Clear();
        foreach (var request in requests) {
            if (!FriendRequests.Contains(request)) {
                FriendRequests.Add(request);
            }
        }

        IsRefreshing = false;
    }

    private async void Accept(string username)
    {
        await _apiService.AcceptFriendRequestAsync(username);
        FriendRequests.Remove(FriendRequests.First(x => x.Username == username));
    }

    private async void Reject(string username)
    {
        await _apiService.RejectFriendRequestAsync(username);
        FriendRequests.Remove(FriendRequests.First(x => x.Username == username));
    }
}
