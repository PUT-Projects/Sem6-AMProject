using Chatter.ViewModels.Dashboard;

namespace Chatter.Views.Dashboard;

public partial class AcceptView : ContentPage
{
	public AcceptView(AcceptViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}