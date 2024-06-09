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
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SkiaSharp;
using System.IO.Compression;

namespace Chatter.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private readonly MessageRepository _repository;
    private readonly IMessageCollector _messageCollector;
    private readonly CryptographyService _cryptoService;
    private readonly object _addMessageLock = new ();
    public string NewMessage { get; set; } = string.Empty;
    public Models.Dashboard.User User { get; set; } = new ();
    public ObservableCollection<Message> Messages { get; } = new ();
    public IAsyncRelayCommand SendMessageCommand { get; }
    public IAsyncRelayCommand SendLocationCommand { get; }
    public IAsyncRelayCommand PickImageCommand { get; }
    public IAsyncRelayCommand LoadMoreMessagesCommand { get; }
    public ICommand BackCommand { get; }
    public CollectionView CollectionView { get; set; }
    public ChatViewModel(IApiService apiService, MessageRepository repository, 
                         IMessageCollector messageCollector, CryptographyService cryptoService)
    {
        _apiService = apiService;
        _repository = repository;
        _messageCollector = messageCollector;
        _cryptoService = cryptoService;
        SendMessageCommand = new AsyncRelayCommand(() => SendMessage(NewMessage.Trim(), Message.MessageType.Text, () => { NewMessage = string.Empty; }));
        SendLocationCommand = new AsyncRelayCommand(SendLocationMessage);
        LoadMoreMessagesCommand = new AsyncRelayCommand(LoadMoreMessages);
        PickImageCommand = new AsyncRelayCommand(PickImage);
    }

    public async Task PickImage()
    {
        try {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo is null) return;

            var stream = await photo.OpenReadAsync();

            var compressedStream = CompressImageAsync(stream);

            var image = new byte[compressedStream.Length];
            await compressedStream.ReadAsync(image, 0, (int)compressedStream.Length);

            await SendMessage(Convert.ToBase64String(image), Message.MessageType.Image);

        }
        catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    public static string CompressAndConvertToBase64(byte[] data)
    {
        using (var memoryStream = new MemoryStream()) {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true)) {
                gzipStream.Write(data, 0, data.Length);
            }
            // Convert the compressed bytes in the MemoryStream to a Base64 string
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    public Stream CompressImageAsync(Stream inputStream, long quality = 90, int maxWidth = 1920, int maxHeight = 1080)
    {
        var original = SKBitmap.Decode(inputStream);

        // Calculate the scale to resize the image
        double scale = Math.Min((double)maxWidth / original.Width, (double)maxHeight / original.Height);
        scale = Math.Min(scale, 1); // Ensure we don't upscale the image

        int newWidth = (int)(original.Width * scale);
        int newHeight = (int)(original.Height * scale);

        // Create a new image with the desired dimensions
        var resizedImage = original.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

        // Compress the image using a loop to reduce the quality until the size is under 2MB
        var memoryStream = new MemoryStream();
        using (SKImage image = SKImage.FromBitmap(resizedImage)) {
            SKData encoded = image.Encode(SKEncodedImageFormat.Jpeg, (int)quality);
            while (encoded.Size > 2 * 1024 * 1024 && quality > 5) // Check size is more than 2MB and quality above 5%
            {
                quality -= 5;
                encoded = image.Encode(SKEncodedImageFormat.Jpeg, (int)quality);
            }
            encoded.SaveTo(memoryStream);
        }

        memoryStream.Position = 0; // Reset the position to the beginning of the stream
        return memoryStream;
    }

    private async Task SendLocationMessage()
    {
        var locationService = new LocationService();
        var location = await locationService.GetCurrentLocationUrlAsync();

        if (location.Item1 is null) {
            var toast = Toast.Make(location.Item2!, ToastDuration.Long, 15);
            await toast.Show();
        } else {
            await SendMessage(location.Item1, Message.MessageType.Text);
        }
    }

    public async void OnLoad()
    {
        _cryptoService.ReceiverXmlKey = await _apiService.GetPublicKey(User.Username);
        _messageCollector.AddObserver(Callback);
        _messageCollector.MuteNotificationsFrom(User.Username);
    }
    public void OnExit()
    {
        _messageCollector.RemoveObserver(Callback);
        _messageCollector.UnMute();
    }

    private async Task SendMessage(string newMessage, Message.MessageType type, Action? Callback = null)
    {
        if (string.IsNullOrWhiteSpace(newMessage)) return;

        var message = CreateMessage(newMessage);
        message.Type = type;

        var result = await _apiService.SendMessageAsync(message);
        if (result == PostMessageDto.Result.PublicKeyExpired) {
            _cryptoService.ReceiverXmlKey = await _apiService.GetPublicKey(User.Username);

            await Task.Delay(50);

            message = CreateMessage(newMessage);
            message.Type = type;
            result = await _apiService.SendMessageAsync(message);
        }

        if (result == PostMessageDto.Result.Failed) {
            var toast = Toast.Make("Couldn't deliver the message!", ToastDuration.Long, 15);
            await toast.Show();
            return;
        }

        var uiMessage = new Message {
            Content = newMessage,
            Sender = _apiService.Username,
            Receiver = message.Receiver,
            TimeStamp = message.TimeStamp,
            Type = type
        };

        Callback?.Invoke();

        await _repository.AddMessage(uiMessage);

        lock (_addMessageLock) {
            Messages.Insert(0, uiMessage);
        }

        await Task.Delay(5);

        CollectionView.ScrollTo(0);

    }

    private PostMessageDto CreateMessage(string newMessage)
    {
        var encryptedMessage = _cryptoService.EncryptMessage(newMessage);

        var message = new PostMessageDto {
            Content = encryptedMessage.Content,
            Key = encryptedMessage.Key,
            IV = encryptedMessage.IV,
            Receiver = User.Username,
            TimeStamp = DateTime.Now,
            Type = Message.MessageType.Text,
            ReceiverPublicKey = encryptedMessage.PublicKey
        };

        return message;
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

    private async Task Callback(IEnumerable<DecryptedMessage> messages)
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
