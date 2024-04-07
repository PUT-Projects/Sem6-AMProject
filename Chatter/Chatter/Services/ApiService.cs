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

        var response = await client.PostAsJsonAsync($"{_settings.ApiUrl}/Account/register", user);

        if (response is null) {
            var toast = Toast.Make("An error occurred while registering the user.", ToastDuration.Long, 15);
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
}
