using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chatter.Entities;

namespace Chatter.Models;

public class GetMessageDto
{
    public string Content { get; set; }
    public string Sender { get; set; }
    public Message.MessageType Type { get; set; }
    public DateTime TimeStamp { get; set; }
}
