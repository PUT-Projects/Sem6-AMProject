using Chatter.Models.Dashboard;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Services;

class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Settings _settings;
    private string _jwtToken = string.Empty;

    public ApiService(IHttpClientFactory httpClientFactory, Settings settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
    }

    public async Task<bool> RegisterUserAsync(Models.Startup.User user)
    {
        using var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/register", user);

        if (response is null) {
            var toast = Toast.Make("An error occurred while registering the user.", ToastDuration.Long);
            await toast.Show();
            return false;
        }

        if (!response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var toast = Toast.Make(content, ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        return true;
    }

    public async Task<bool> LoginUserAsync(Models.Startup.User user)
    {
        using var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/login", user);

        if (response is null) {
            var toast = Toast.Make("An error occurred while logging in the user.", ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) {
            var toast = Toast.Make(content, ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        _jwtToken = "Bearer " + content;
        return true;
    }

    public async Task<IEnumerable<User>> GetFriendsAsync()
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);
        try {
            var friends = await client.GetFromJsonAsync<IEnumerable<User>>($"{_settings.ApiUrl}/account/friends");

            return friends ?? [];
        } catch (Exception ex) {
            var toast = Toast.Make("An error occurred while fetching the friends.", ToastDuration.Long, 15);
            await toast.Show();
            return [];
        }
    }

    public async Task<bool> InviteFriendAsync(User friend)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/friends/invite", friend);

        if (response is null) {
            var toast = Toast.Make("An error occurred while inviting the friend.", ToastDuration.Long, 15);
            await toast.Show();
            return false;
        }

        if (!response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var toast = Toast.Make(content, ToastDuration.Long);
            await toast.Show();
            return false;
        }

        return true;
    }

    public async Task<bool> AcceptFriendAsync(User friend)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/friends/accept", friend.Username);

        if (response is null) {
            var toast = Toast.Make("An error occurred while accepting the friend request.", ToastDuration.Long);
            await toast.Show();
            return false;
        }

        if (!response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var toast = Toast.Make(content, ToastDuration.Short);
            await toast.Show();
            return false;
        }

        return true;
    }

    public async Task Logout()
    {
        //using var client = _httpClientFactory.CreateClient();
        //ConfigureHttpClient(client);

        //var response = await client.PostAsync($"{_settings.ApiUrl}/account/logout", null);

        //if (response is null) {
        //    var toast = Toast.Make("An error occurred while logging out.", ToastDuration.Long);
        //    await toast.Show();
        //    return;
        //}

        //if (!response.IsSuccessStatusCode) {
        //    var content = await response.Content.ReadAsStringAsync();
        //    var toast = Toast.Make(content, ToastDuration.Long);
        //    await toast.Show();
        //    return;
        //}

        _jwtToken = string.Empty;
    }

    private void ConfigureHttpClient(HttpClient client)
    {
        if (string.IsNullOrEmpty(_jwtToken)) {
            throw new Exception("The JWT token is missing. Please login first.");
        }

        client.DefaultRequestHeaders.Add("Authorization", _jwtToken);
    }
}
