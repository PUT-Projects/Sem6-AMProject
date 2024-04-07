namespace ChatterAPI.Models
{
    public class MessageDto
    {
        public string Content { get; set; }
        public string Receiver { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
