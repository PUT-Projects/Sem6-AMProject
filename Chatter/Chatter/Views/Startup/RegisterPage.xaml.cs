using Chatter.ViewModels.Startup;

namespace Chatter.Views.Startup;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
		var viewModel = (RegisterPageViewModel)BindingContext;
		viewModel.BackCommand.Execute(null);
		return true;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
		var viewModel = (RegisterPageViewModel)BindingContext;
		viewModel.UpdateColors(entryFrame, entryIconFrame, entryIcon);
    }
}