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
using CommunityToolkit.Mvvm.Input;
using System.Runtime.CompilerServices;
using Chatter.Entities;

namespace Chatter.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private readonly MessageRepository _repository;
    private readonly IMessageCollector _messageCollector;
    private readonly object _addMessageLock = new ();
    public string NewMessage { get; set; } = string.Empty;
    public Models.Dashboard.User User { get; set; } = new ();
    public ObservableCollection<Message> Messages { get; } = new ();
    public IAsyncRelayCommand SendMessageCommand { get; }
    public IAsyncRelayCommand LoadMoreMessagesCommand { get; }
    public ICommand BackCommand { get; }
    public CollectionView CollectionView { get; set; }
    public ChatViewModel(IApiService apiService, MessageRepository repository, IMessageCollector messageCollector)
    {
        _apiService = apiService;
        _repository = repository;
        _messageCollector = messageCollector;
        SendMessageCommand = new AsyncRelayCommand(SendMessage);
        LoadMoreMessagesCommand = new AsyncRelayCommand(LoadMoreMessages);
    }
    public void OnLoad()
    {
        _messageCollector.AddObserver(Callback);
        _messageCollector.MuteNotificationsFrom(User.Username);
    }
    public void OnExit()
    {
        _messageCollector.RemoveObserver(Callback);
        _messageCollector.UnMute();
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage)) return;

        var message = new PostMessageDto {
            Content = NewMessage,
            Receiver = User.Username,
            TimeStamp = DateTime.Now,
            Type = Message.MessageType.Text
        };

        await _apiService.SendMessageAsync(message);

        NewMessage = string.Empty;

        var uiMessage = new Message {
            Content = message.Content,
            Sender = _apiService.Username,
            Receiver = message.Receiver,
            TimeStamp = message.TimeStamp
        };

        await _repository.AddMessage(uiMessage);

        lock (_addMessageLock) {
            Messages.Insert(0, uiMessage);
        }

        await Task.Delay(5);

        CollectionView.ScrollTo(0);

    }
    private async Task LoadMoreMessages()
    {
        var messages = await _repository.GetMessagesAsync(User.Username, Messages.Count + 50);

        if (messages.Count() == Messages.Count) return;

        lock (_addMessageLock) {
            Messages.Clear();

            foreach (var message in messages) {
                Messages.Add(message);
            }
        }
    }

    private async Task Callback(IEnumerable<GetMessageDto> messages)
    {
        var uiMessages = messages.Select(m => new Message {
            Content = m.Content,
            Sender = m.Sender,
            Receiver = _apiService.Username,
            TimeStamp = m.TimeStamp,
            Type = m.Type
        });

        MainThread.BeginInvokeOnMainThread(() => {
            lock (_addMessageLock) {
                foreach (var message in uiMessages) {
                    Messages.Insert(0, message);
                }
            }

            CollectionView.ScrollTo(0);
        });
    }


}
