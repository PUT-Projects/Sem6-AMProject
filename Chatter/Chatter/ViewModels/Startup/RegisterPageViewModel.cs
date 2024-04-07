using Chatter.Models.Startup;
using Chatter.Views.Dashboard;
using Chatter.Views.Startup;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels.Startup;

public sealed class RegisterPageViewModel : ViewModelBase
{
    public User User { get; set; }
    public ICommand RegisterCommand { get; }
    public ICommand BackCommand { get; }

    public RegisterPageViewModel()
    {
        User = new User();
        RegisterCommand = new Command(Register);
        BackCommand = new Command(OnBackButtonPressed);
    }

    private async void Register()
    {
        var toast = Toast.Make(User.Username, ToastDuration.Short);
        await toast.Show();
    }

    private async void OnBackButtonPressed()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

}
