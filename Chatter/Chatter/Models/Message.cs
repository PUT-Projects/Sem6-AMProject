using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models;
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
}
