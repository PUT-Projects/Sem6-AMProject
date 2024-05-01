using Chatter.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models;

public class DecryptedMessage
{
    public string Content { get; set; }
    public string Sender { get; set; }
    public Message.MessageType Type { get; set; }
    public DateTime TimeStamp { get; set; }
}
