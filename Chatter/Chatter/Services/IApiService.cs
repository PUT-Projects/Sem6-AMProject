namespace Chatter.Services;

public interface IApiService
{
    Task<bool> LoginUserAsync(Models.Startup.User user);
    Task<bool> RegisterUserAsync(Models.Startup.User user);
    Task<bool> AcceptFriendAsync(Models.Dashboard.User friend);
    Task<IEnumerable<Models.Dashboard.User>> GetFriendsAsync();
    Task<bool> InviteFriendAsync(Models.Dashboard.User friend);
    Task Logout();
}