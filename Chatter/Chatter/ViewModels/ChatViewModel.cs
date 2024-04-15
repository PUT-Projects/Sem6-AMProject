using Chatter.Repositories;
using Chatter.Services;
using Chatter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatter.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private readonly MessageRepository _repository;
    public INavigation Navigation { get; set; }
    public string NewMessage { get; set; } = string.Empty;
    public Models.Dashboard.User User { get; set; } = new ();
    public ObservableCollection<Message> Messages { get; } = [];
    public ICommand SendMessageCommand { get; }
    public ICommand BackCommand { get; }

    public ChatViewModel(IApiService apiService)
    {
        _apiService = apiService;
       // _repository = repository;
        
        SendMessageCommand = new Command(SendMessage);
        AddMockMessages();
    }



    private async void SendMessage()
    {
        // 
    }

    private void AddMockMessages()
    {
        Messages.Clear();
        Messages.Add(new Message { Content = "Hello", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Hi", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "How are you?", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "I'm good, thanks!", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "What are you up to?", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Just reading a book", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Cool, which one?", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "The one you recommended", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Oh, that one is great!", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "I know, right?", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "I'm glad you like it", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Thanks for the recommendation", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "You're welcome", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "I have to go now", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Talk to you later", Sender = _apiService.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Bye", Sender = User.Username, TimeStamp = DateTime.Now });
        Messages.Add(new Message { Content = "Bye", Sender = _apiService.Username, TimeStamp = DateTime.Now });
    }
}
