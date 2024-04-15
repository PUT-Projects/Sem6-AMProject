namespace Chatter.Models.Dashboard;

public class AcceptUser
{
    public string Username { get; set; }
    public DateTime TimeStamp { get; set; }
    public string ImageUrl => ProfilePictures.GetPictureFromUsername(Username);
}
