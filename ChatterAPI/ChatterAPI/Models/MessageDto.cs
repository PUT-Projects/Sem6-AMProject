using ChatterAPI.Entities;

namespace ChatterAPI.Models
{
    public class MessageDto
    {
        public string Content { get; set; }
        public string Key { get; set; }
        public string IV { get; set; }
        public string Receiver { get; set; }
        public Message.MessageType Type { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
