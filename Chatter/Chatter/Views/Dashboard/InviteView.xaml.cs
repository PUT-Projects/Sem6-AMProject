using Chatter.ViewModels.Dashboard;

namespace Chatter.Views.Dashboard;

public partial class InviteView : ContentPage
{
    private InviteViewModel _viewModel { get => (InviteViewModel)BindingContext; }
	public InviteView(InviteViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _viewModel.OnAppearing(searchBar);
    }

    protected override bool OnBackButtonPressed()
    {
        _viewModel.BackCommand.Execute(null);
        return true;
    }
}