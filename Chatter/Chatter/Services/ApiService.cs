using Chatter.Models;
using Chatter.Models.Dashboard;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Services;

class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Settings _settings;
    private string _jwtToken = string.Empty;
    public string Username { get; private set; } = string.Empty;

    public ApiService(IHttpClientFactory httpClientFactory, Settings settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
    }

    public async Task<bool> RegisterUserAsync(Models.Startup.User user)
    {
        using var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/register", user);

        return await HandleResponse(response, "An error occurred while registering the user.");
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
        Username = user.Username;
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

    public async Task<bool> InviteFriendAsync(string friend)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var body = new { Username = friend, TimeStamp = DateTime.Now };

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/friends/invite", body);

        return await HandleResponse(response, "An error occurred while inviting the friend.");
    }

    public async Task<bool> AcceptFriendRequestAsync(string username)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/friends/accept", username);

        return await HandleResponse(response, "An error occurred while accepting the friend request.");
    }
    public async Task<bool> RejectFriendRequestAsync(string username)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/account/friends/reject", username);

        return await HandleResponse(response, "An error occurred while rejecting the friend request.");
    }

    /*
     * Handles the response from the server.
     *
     * @param response The response from the server.
     * returns True if the response is successful, False otherwise.
     */
    private async Task<bool> HandleResponse(HttpResponseMessage? response, string errorMessage)
    {
        if (response is null) {
            var toast = Toast.Make(errorMessage, ToastDuration.Long);
            await toast.Show();
            return false;
        }

        if (!response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            Device.BeginInvokeOnMainThread(async () => {
                var toast = Toast.Make(content, ToastDuration.Short);
                await toast.Show();
            });


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
        client.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<(IEnumerable<string> friends, IEnumerable<SearchUser> users)> SearchFriendsAndUsersAsync(string searchQuery)
    {
        var results = await Task.WhenAll(
            Task.Run(async () => (object)await SearchFriendsAsync(searchQuery)),
            Task.Run(async () => (object)await SearchUsersAsync(searchQuery))
        );

        return ((IEnumerable<string>)results[0], (IEnumerable<SearchUser>)results[1]);
    }
    public async Task<IEnumerable<string>> SearchFriendsAsync(string searchQuery)
    {
        string query = $"{_settings.ApiUrl}/account/friends/search";
        return await SearchUsersTemplateAsync<string>(searchQuery, query);
    }

    public async Task<IEnumerable<SearchUser>> SearchUsersAsync(string searchQuery)
    {
        string query = $"{_settings.ApiUrl}/account/users/search";
        return await SearchUsersTemplateAsync<SearchUser>(searchQuery, query);
    }

    private async Task<IEnumerable<T>> SearchUsersTemplateAsync<T>(string searchQuery, string query)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var response = await client.PostAsJsonAsync(query, searchQuery);

        if (response is null) {
            var toast = Toast.Make("An error occurred while searching for users.", ToastDuration.Long);
            await toast.Show();
            return [];
        }

        if (!response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var toast = Toast.Make(content, ToastDuration.Long);
            await toast.Show();
            return [];
        }

        var users = await response.Content.ReadFromJsonAsync<IEnumerable<T>>();

        return users ?? [];
    }

    public async Task<IEnumerable<AcceptUser>> GetFriendRequests()
    {
        return await GetIEnumerableAsync<AcceptUser>($"{_settings.ApiUrl}/account/friends/requests");
    }

    public async Task<IEnumerable<GetMessageDto>> GetNewMessagesAsync()
    {
        return await GetIEnumerableAsync<GetMessageDto>($"{_settings.ApiUrl}/chatting/messages");
    }

    private async Task<IEnumerable<T>> GetIEnumerableAsync<T>(string url)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        try {
            var data = await client.GetFromJsonAsync<IEnumerable<T>>(url);

            return data ?? [];
        } 
        catch (Exception ex) {
            var toast = Toast.Make($"An error occurred while fetching the {nameof(IEnumerable<T>)}.", ToastDuration.Long);
            await toast.Show();
            return [];
        }
    }

    public async Task<bool> SendMessageAsync(PostMessageDto message)
    {
        using var client = _httpClientFactory.CreateClient();
        ConfigureHttpClient(client);

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/chatting/message", message);

        return await HandleResponse(response, "An error occurred while sending the message.");
    }
}
