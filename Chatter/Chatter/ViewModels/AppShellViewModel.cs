#if ANDROID
using Chatter.Platforms.Android;
#endif
using Chatter.Services;
using Chatter.Views.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels;

public class AppShellViewModel : ViewModelBase
{
    private readonly IApiService _apiService;

    private readonly IMessageCollector _messageCollector;
    public ICommand LogoutCommand { get; }
    public AppShellViewModel(IApiService apiService, IMessageCollector messageCollector)
    {
        _apiService = apiService;
        _messageCollector = messageCollector;
        LogoutCommand = new Command(Logout);
    }

    private async void Logout()
    {
        _messageCollector.Stop();

        await _apiService.Logout();
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
    }
}
