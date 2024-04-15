using Chatter.ViewModels;

namespace Chatter.Views;

public partial class ChatView : ContentPage
{
	public string Param { get; set; } = string.Empty;
	public ChatView(ChatViewModel viewModel)
	{
		InitializeComponent();
		viewModel.Navigation = Navigation;
		BindingContext = viewModel;
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{

    }
}