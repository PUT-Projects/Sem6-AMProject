using Chatter.ViewModels.Startup;

namespace Chatter.Views.Startup;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}