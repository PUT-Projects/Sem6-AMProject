using Chatter.Models.Dashboard;
using Chatter.Services;
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
    private readonly IApiService _apiService;
    private readonly BackgroundWorker _worker;
    public ObservableCollection<AcceptUser> FriendRequests { get; } = new();
    public ICommand AcceptCommand { get; }
    public ICommand RejectCommand { get; }
    public ICommand RefreshCommand { get; }
    public bool IsRefreshing { get; set; }
    public AcceptViewModel(IApiService apiService)
    {
        _apiService = apiService;
        _worker = new BackgroundWorker();

        AcceptCommand = new Command<string>(Accept);
        RejectCommand = new Command<string>(Reject);
        RefreshCommand = new Command(async () => await RefreshAsync());
        IsRefreshing = false;
        
        FriendRequests.Add(new AcceptUser { Username = "JohnDoe" });
        FriendRequests.Add(new AcceptUser { Username = "JaneDoe" });
        FriendRequests.Add(new AcceptUser { Username = "JohnSmith" });
        FriendRequests.Add(new AcceptUser { Username = "Alice" });
    }

    public void RunApiService()
    {
        var t = Task.Factory.StartNew(RefreshLoop, TaskCreationOptions.LongRunning);
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
            var user = new AcceptUser { Username = request };
            if (!FriendRequests.Contains(user)) {
                FriendRequests.Add(user);
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
