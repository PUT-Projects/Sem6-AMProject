using Chatter.Entities;
using Chatter.Models;
using Chatter.Repositories;
using Chatter.Services;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Platforms.Android;

public class AndroidMessageCollector : IMessageCollector
{
    private readonly IApiService _apiService;
    private readonly MessageRepository _repository;
    private readonly CryptographyService _cryptoService;

    private readonly List<Func<IEnumerable<DecryptedMessage>, Task>> _observers = new();
    public AndroidMessageCollector(IApiService apiService, MessageRepository repository, CryptographyService cryptoService)
    {
        _apiService = apiService;
        _repository = repository;
        _cryptoService = cryptoService;
        AndroidServiceManager.MessageCollector = this;
    }

    public void AddObserver(Func<IEnumerable<DecryptedMessage>, Task> observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(Func<IEnumerable<DecryptedMessage>, Task> observer)
    {
        _observers.Remove(observer);
    }

    public void StartCollectingMessages()
    {
        _repository.UpdateConnection();
        _cryptoService.UpdateRSA();
        AndroidServiceManager.Start();
    }

    public void Stop()
    {
        AndroidServiceManager.Stop();
    }

    public async Task CollectAndNotify()
    {
        var messages = await _apiService.GetNewMessagesAsync();
        if (messages.Count() == 0) return;

        var decryptedMessages = DecryptMessages(messages);  

        await UpdateDatabase(decryptedMessages);

        await NotifyObservers(decryptedMessages);
    }

    private async Task NotifyObservers(IEnumerable<DecryptedMessage> messages)
    {
        foreach (var observer in _observers) {
            await observer(messages);
        }
    }

    private async Task UpdateDatabase(IEnumerable<DecryptedMessage> dtos)
    {
        var messages = dtos.Select(dto => new Message {
            Content = dto.Content,
            Sender = dto.Sender,
            Receiver = _apiService.Username,
            TimeStamp = dto.TimeStamp,
            Type = dto.Type
        });

        await _repository.AddMessages(messages);
    }

    public void MuteNotificationsFrom(string username)
    {
        AndroidServiceManager.MutedUser = username;
    }
    public void UnMute()
    {
        AndroidServiceManager.MutedUser = string.Empty;
    }

    private IEnumerable<DecryptedMessage> DecryptMessages(IEnumerable<GetMessageDto> dtos)
    {
        return dtos.Select(dto => new DecryptedMessage {
            Content = _cryptoService.DecryptMessage(dto.Key, dto.IV, dto.Content),
            Sender = dto.Sender,
            TimeStamp = dto.TimeStamp,
            Type = dto.Type
        });
    }
}
