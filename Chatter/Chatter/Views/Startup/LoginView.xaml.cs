using Chatter.ViewModels.Startup;

namespace Chatter.Views.Startup;

public partial class LoginView : ContentPage
{
    private LoginViewModel ViewModel => (BindingContext as LoginViewModel)!;
    public LoginView(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        if (ViewModel.RunAnimationCommand.CanExecute(LogoImage)) {
            ViewModel.RunAnimationCommand.Execute(LogoImage);
        }
    }
}