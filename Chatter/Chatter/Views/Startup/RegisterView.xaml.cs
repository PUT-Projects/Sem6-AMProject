using Chatter.ViewModels.Startup;

namespace Chatter.Views.Startup;

public partial class RegisterView : ContentPage
{
	public RegisterView(RegisterViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
		var viewModel = (RegisterViewModel)BindingContext;
		viewModel.BackCommand.Execute(null);
		return true;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
		var viewModel = (RegisterViewModel)BindingContext;
		viewModel.UpdateColors(entryFrame, entryIconFrame, entryIcon);
    }
}