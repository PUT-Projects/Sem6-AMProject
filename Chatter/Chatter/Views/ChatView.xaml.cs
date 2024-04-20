using Chatter.Models.Dashboard;
using Chatter.ViewModels;

namespace Chatter.Views;

public partial class ChatView : ContentPage
{
	public ChatView(ChatViewModel viewModel)
	{
		InitializeComponent();
		viewModel.CollectionView = collectionView;
		BindingContext = viewModel;

	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		var vm = (ChatViewModel)BindingContext;
		//vm.ScrollToBottom();
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