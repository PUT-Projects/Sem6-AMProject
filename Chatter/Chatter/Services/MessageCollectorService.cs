using Chatter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// import community toolkit
using CommunityToolkit.Maui.Alerts;
using System.Diagnostics;
using Chatter.Models;

namespace Chatter.Services;

public class MessageCollectorService
{
    private readonly IApiService _apiService;
    private readonly MessageRepository _repository;
    private readonly object _lock = new ();
    private bool _isRunning = false;
    private Task? _loop = null;
    private List<Func<IEnumerable<GetMessageDto>, Task>> _observers = new ();
    public MessageCollectorService(IApiService apiService, MessageRepository repository)
    {
        _apiService = apiService;
        _repository = repository;
    }

    public void AddObserver(Func<IEnumerable<GetMessageDto>, Task> observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(Func<IEnumerable<GetMessageDto>, Task> observer)
    {
        _observers.Remove(observer);
    }

    public void StartCollectingMessages()
    {
        if (_isRunning) {
            Stop();
        }

        _loop = Task.Factory.StartNew(CollectingLoop, TaskCreationOptions.LongRunning);
    }

    private async Task CollectingLoop()
    {
        lock (_lock) {
            if (_isRunning) return;
        }

        _isRunning = true;

        _repository.UpdateConnection();

        while (true) {

            lock (_lock) {
                if (!_isRunning) break;
            }

            var messages = await _apiService.GetNewMessagesAsync();

            foreach (var dto in messages) {
                var message = new Message {
                    Content = dto.Content,
                    Sender = dto.Sender,
                    Receiver = _apiService.Username,
                    TimeStamp = dto.TimeStamp,
                    Type = dto.Type
                };

                await _repository.AddMessage(message);

            }
            await Task.Delay(1000);

            if (messages.Count() > 0) { 
                await NotifyObservers(messages);
            }
        }
    }

    public void Stop()
    {
        lock (_lock) {
            _isRunning = false;
        }

        _loop?.Wait();

        _loop = null;
    }
    
    private async Task NotifyObservers(IEnumerable<GetMessageDto> dto)
    {
        foreach (var observer in _observers) {
            await observer(dto);
        }
    }
}
