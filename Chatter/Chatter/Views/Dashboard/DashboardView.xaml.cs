using Chatter.ViewModels.Dashboard;

namespace Chatter.Views.Dashboard;

public partial class DashboardView : ContentPage
{
	public DashboardView(DashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
		var vm = (DashboardViewModel)BindingContext;
		vm.RefreshCommand.Execute(null);
    }
}