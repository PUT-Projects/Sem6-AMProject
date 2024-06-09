using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Entities;
[Table("Messages")]
public class Message
{
    public enum MessageType
    {
        Text,
        Image
    }
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Content { get; set; }
    [NotNull]
    public MessageType Type { get; set; }
    [Indexed, NotNull]
    public string Sender { get; set; }
    [Indexed, NotNull]
    public string Receiver { get; set; }
    [NotNull]
    public DateTime TimeStamp { get; set; }

    [Ignore]
    public IAsyncRelayCommand CopyCommand => new AsyncRelayCommand(async () => {
        await Clipboard.SetTextAsync(Content);
    });

    [Ignore]
    public ImageSource ImageSource =>
            ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(Content)));
        
}
