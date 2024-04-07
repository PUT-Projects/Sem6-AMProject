using Chatter.ViewModels.Dashboard;

namespace Chatter.Views.Dashboard;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}