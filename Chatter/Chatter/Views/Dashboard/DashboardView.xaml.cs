using Chatter.ViewModels.Dashboard;

namespace Chatter.Views.Dashboard;

public partial class DashboardView : ContentPage
{
	public DashboardView(DashboardViewModel viewModel)
	{
		InitializeComponent();
		viewModel.Navigation = Navigation;
		BindingContext = viewModel;
	}
}