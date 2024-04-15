namespace ChatterAPI.Entities;

public class FriendPair
{
    public enum Status
    {
        Invited,
        Friends,
    }
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public Status FriendshipStatus { get; set; }
    public DateTime TimeStamp { get; set; }
}
