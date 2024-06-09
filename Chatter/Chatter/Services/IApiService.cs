using Chatter.Models;
using Chatter.Models.Dashboard;

namespace Chatter.Services;

public interface IApiService
{
    string Username { get; }
    Task<bool> LoginUserAsync(Models.Startup.User user);
    Task<bool> RegisterUserAsync(Models.Startup.RegisterUser user);
    Task<bool> AcceptFriendRequestAsync(string username);
    Task<bool> RejectFriendRequestAsync(string username);
    Task<IEnumerable<User>> GetFriendsAsync();
    Task<bool> InviteFriendAsync(string friend);
    Task Logout();
    Task<(IEnumerable<string> friends, IEnumerable<SearchUser> users)> SearchFriendsAndUsersAsync(string searchQuery);
    Task<IEnumerable<string>> SearchFriendsAsync(string searchQuery);
    Task<IEnumerable<SearchUser>> SearchUsersAsync(string searchQuery);
    Task<IEnumerable<AcceptUser>> GetFriendRequests();
    Task<IEnumerable<GetMessageDto>> GetNewMessagesAsync();
    Task<PostMessageDto.Result> SendMessageAsync(PostMessageDto message);
    Task<string> GetPublicKey(string username);
    Task PostPublicKeyAsync(string publicKeyXml);
}