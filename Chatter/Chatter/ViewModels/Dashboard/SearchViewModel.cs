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

namespace Chatter.ViewModels.Dashboard
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private string _searchQuery = string.Empty;
        public ICommand BackCommand { get; }

        public ObservableCollection<User> Friends { get; } = new();

        public string SearchQuery { get => _searchQuery; set => Search(value); }

        public SearchViewModel(IApiService apiService)
        {
            _apiService = apiService;
            BackCommand = new Command(GoBack);
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

        private async void GoBack()
        {
            await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
        }
    }
}
