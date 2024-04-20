using Chatter.Services;
using Chatter.Models.Dashboard;
using Chatter.Views.Dashboard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Chatter.Views;
using CommunityToolkit.Maui.Alerts;

namespace Chatter.ViewModels.Dashboard
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private string _searchQuery = string.Empty;
        public IAsyncRelayCommand<string> SelectedCommand { get; }
        public ObservableCollection<User> Friends { get; } = new();

        public string SearchQuery { get => _searchQuery; set => Search(value); }

        public SearchViewModel(IApiService apiService)
        {
            _apiService = apiService;
            SelectedCommand = new AsyncRelayCommand<string>(UserSelected);
        }

        private async Task UserSelected(string? username)
        {
            if (string.IsNullOrWhiteSpace(username)) return;

            var chatView = ChatView.Create(username);

            if (chatView == null) {
                var toast = Toast.Make("ChatView not found");
                await toast.Show();
                return;
            }
            
            await Shell.Current.Navigation.PushAsync(chatView);
        }

        public void OnAppearing(SearchBar searchBar)
        {
            bool ok = searchBar.Focus();
        }

        private async void Search(string newValue)
        {
            if (_searchQuery == newValue) return;
            
            _searchQuery = newValue;

            if (string.IsNullOrWhiteSpace(_searchQuery)) {
                Friends.Clear();
                return;
            }

            var friends = await _apiService.SearchFriendsAsync(_searchQuery);

            Friends.Clear();
            foreach (var username in friends) {
                var friend = new User { Username = username };
                Friends.Add(friend);
            }
        }
    }
}
