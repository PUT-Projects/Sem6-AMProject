using Chatter.Models.Startup;

namespace Chatter.Services;

public interface IApiService
{
    Task<bool> LoginUserAsync(User user);
    Task<bool> RegisterUserAsync(User user);
}