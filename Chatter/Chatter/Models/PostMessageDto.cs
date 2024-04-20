using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models;

public class PostMessageDto
{
    public string Content { get; set; }
    public string Receiver { get; set; }
    public Message.MessageType Type { get; set; }
    public DateTime TimeStamp { get; set; }
}
