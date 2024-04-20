namespace ChatterAPI.Entities;

public class Message
{
    public enum MessageType
    {
        Text,
        Image
    }
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
    public MessageType Type { get; set; }
    public DateTime TimeStamp { get; set; }
}
