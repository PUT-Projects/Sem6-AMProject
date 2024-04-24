using Chatter.Models.Dashboard;
using Chatter.ViewModels;

namespace Chatter.Views;

public partial class ChatView : ContentPage
{
    private ChatViewModel _viewModel => (ChatViewModel)BindingContext;
	public ChatView(ChatViewModel viewModel)
	{
		InitializeComponent();
		viewModel.CollectionView = collectionView;
		BindingContext = viewModel;
	}
    protected override void OnDisappearing()
    {
        _viewModel.OnExit();
    }
    protected override void OnAppearing()
	{
        _viewModel.OnLoad();
    }

    public static ChatView? Create(string username)
    {
        var chatView = Application.Current!.MainPage!
            .Handler!.MauiContext!.Services.GetService<ChatView>();

        if (chatView is null) return null;

        var vm = (ChatViewModel)chatView.BindingContext;
        vm.User = new User { Username = username };

        vm.LoadMoreMessagesCommand.Execute(username);
        return chatView;
    }
}