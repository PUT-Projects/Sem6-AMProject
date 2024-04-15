namespace Chatter.Models.Dashboard;

public class SearchItem
{
    public string Username { get; set; } = string.Empty;
    public bool IsFriend { get; set; } = false;
    public string ImageUrl => ProfilePictures.GetPictureFromUsername(Username);
    public string Label { get; set; } = string.Empty;
    public bool IsLabel { get; set; } = false;
}
