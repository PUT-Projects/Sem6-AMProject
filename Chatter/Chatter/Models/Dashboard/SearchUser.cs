namespace Chatter.Models.Dashboard;

public class SearchUser
{
    public string Username { get; set; }
    public bool IsInvited { get; set; }
    public string ImageUrl => ProfilePictures.GetPictureFromUsername(Username);
    public string InviteImg => IsInvited ? "check.png" : "invite.png";
}
