using ChatterAPI.Entities;
using ChatterAPI.Exceptions;
using ChatterAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatterAPI.Services;

public class AccountService
{
    private readonly ChatterContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;

    public AccountService(ChatterContext context, 
                          IPasswordHasher<User> passwordHasher, 
                          AuthenticationSettings authenticationSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
    }

    private async Task<User> AuthenticateUser(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user is null) {
            throw new UnauthorizedException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

        if (result == PasswordVerificationResult.Failed) {
            throw new UnauthorizedException("Invalid username or password");
        }

        return user;
    }

    public async Task RegisterUser(RegisterDto registerDto)
    {
        var user = new User {
            Username = registerDto.Username,
        };

        var hash = _passwordHasher.HashPassword(user, registerDto.Password);
        user.PasswordHash = hash;

        if (await _context.Users.AnyAsync(u => u.Username == user.Username)) {
            throw new ConflictException("Username is already taken");
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GenerateJWT(LoginDto dto)
    {
        var user = await AuthenticateUser(dto);

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

        var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims, expires: expires, signingCredentials: cred);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public async Task<User> GetUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) {
            throw new BadRequestException("User not found");
        }

        return user;
    }

    public async Task<User> GetUser(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) {
            throw new BadRequestException("User not found");
        }

        return user;
    }

    public async Task<IEnumerable<string>> GetFriends(Guid userId)
    {
        var friendIds = await _context.FriendPairs
            .Where(fp => fp.UserId == userId && fp.FriendshipStatus == FriendPair.Status.Friends)
            .Select(fp => fp.FriendId).ToListAsync();

        return await _context.Users.Where(u => friendIds.Contains(u.Id)).Select(u => u.Username).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetFriendRequests(Guid userId)
    {
        var friendPairs = await _context.FriendPairs
            .Where(fp => fp.FriendId == userId && fp.FriendshipStatus == FriendPair.Status.Invited)
            .ToListAsync();

        var friendIds = friendPairs.Select(fp => fp.UserId);
        return await _context.Users.Where(u => friendIds.Contains(u.Id)).ToListAsync();
    }

    public async Task AddFriend(Guid userId, string friendUsername)
    {
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Username == friendUsername);
        if (friend is null) {
            throw new BadRequestException("User not found");
        }

        if (userId == friend.Id) {
            throw new BadRequestException("Cannot add yourself as a friend");
        }

        var fp = await _context.FriendPairs.FirstOrDefaultAsync(fp => fp.UserId == userId && fp.FriendId == friend.Id);
        if (fp is not null) {
            if (fp.FriendshipStatus == FriendPair.Status.Friends) {
                throw new BadRequestException("Already friends");
            } else {
                throw new BadRequestException("Friend request already sent");
            }
        }

        var friendPair = new FriendPair {
            UserId = userId,
            FriendId = friend.Id,
            FriendshipStatus = FriendPair.Status.Invited,
        };

        await _context.FriendPairs.AddAsync(friendPair);
        await _context.SaveChangesAsync();
    }

    public async Task AcceptFriendRequest(Guid userId, string friendUsername)
    {
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Username == friendUsername);
        if (friend is null) {
            throw new BadRequestException("User not found!");
        }

        var friendPair = await _context.FriendPairs
            .FirstOrDefaultAsync(fp => fp.UserId == friend.Id && fp.FriendId == userId);
        if (friendPair is null) {
            throw new BadRequestException("Friend request not found");
        }

        if (friendPair.FriendshipStatus == FriendPair.Status.Friends) {
            throw new BadRequestException("Already friends!");
        }

        friendPair.FriendshipStatus = FriendPair.Status.Friends;

        var reversePair = new FriendPair {
            UserId = userId,
            FriendId = friend.Id,
            FriendshipStatus = FriendPair.Status.Friends
        };

        await _context.FriendPairs.AddAsync(reversePair);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFriend(Guid userId, string friendUsername)
    {
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Username == friendUsername);
        if (friend is null) {
            return;
        }

        var friendPair = await _context.FriendPairs
            .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.FriendId == friend.Id);
        if (friendPair is null) {
            return;
        }

        _context.FriendPairs.Remove(friendPair);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsFriend(Guid userId, Guid friendId)
    {
        return await _context.FriendPairs
            .AnyAsync(fp => fp.UserId == userId && fp.FriendId == friendId && fp.FriendshipStatus == FriendPair.Status.Friends);
    }

    public async Task<bool> IsFriendRequestSent(Guid userId, Guid friendId)
    {
        return await _context.FriendPairs
            .AnyAsync(fp => fp.UserId == userId && fp.FriendId == friendId && fp.FriendshipStatus == FriendPair.Status.Invited);
    }

    public async Task<bool> IsFriendRequestReceived(Guid userId, Guid friendId)
    {
        return await _context.FriendPairs
            .AnyAsync(fp => fp.UserId == friendId && fp.FriendId == userId && fp.FriendshipStatus == FriendPair.Status.Invited);
    }

    public async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> UserExists(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> FriendPairExists(Guid userId, Guid friendId)
    {
        return await _context.FriendPairs.AnyAsync(fp => fp.UserId == userId && fp.FriendId == friendId);
    }

    public async Task<bool> FriendPairExists(Guid userId, Guid friendId, FriendPair.Status status)
    {
        return await _context.FriendPairs.AnyAsync(fp => fp.UserId == userId && fp.FriendId == friendId && fp.FriendshipStatus == status);
    }
}
