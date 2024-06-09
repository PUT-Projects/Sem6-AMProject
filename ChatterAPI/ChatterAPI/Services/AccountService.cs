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
        CheckUserFields(registerDto);

        var user = new User {
            Username = registerDto.Username,
            PublicKey = registerDto.PublicKey
        };

        string hash = _passwordHasher.HashPassword(user, registerDto.Password);
        user.PasswordHash = hash;

        if (await _context.Users.AnyAsync(u => u.Username == user.Username)) {
            throw new ConflictException("Username is already taken");
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    private void CheckUserFields(RegisterDto registerDto)
    {
        if (registerDto.Username.Length < 3) {
            throw new BadRequestException("Username is too short!");
        }

        if (registerDto.Password.Length > 20) {
            throw new BadRequestException("Username is too long!");
        }

        if (registerDto.Password.Length < 6) {
            throw new BadRequestException("Password is too short!");
        }
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

    public async Task<IEnumerable<FriendDto>> GetFriends(Guid userId)
    {
        var friends = await _context.Users
            .Where(u => _context.FriendPairs
                .Where(fp => fp.UserId == userId && fp.FriendshipStatus == FriendPair.Status.Friends)
                .Select(fp => fp.FriendId)
                .Contains(u.Id))
            .Select(u => new FriendDto { Username = u.Username })
            .ToListAsync();

        return friends;
    }

    public async Task<IEnumerable<FriendRequestDto>> GetFriendRequests(Guid userId)
    {
        var friendRequests = await _context.Users
            .Where(u => _context.FriendPairs
                .Any(fp => fp.FriendId == userId && fp.UserId == u.Id && fp.FriendshipStatus == FriendPair.Status.Invited))
            .Select(u => new FriendRequestDto { Username = u.Username })
            .ToListAsync();

        return friendRequests;
    }


    public async Task AddFriend(Guid userId, FriendRequestDto user)
    {
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
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

        // check if the friend has already sent a request
        var reversePair = await _context.FriendPairs.FirstOrDefaultAsync(fp => fp.UserId == friend.Id && fp.FriendId == userId);
        if (reversePair is not null) {
            await AcceptFriendRequest(userId, user.Username);
            return;
        }

        var friendPair = new FriendPair {
            UserId = userId,
            FriendId = friend.Id,
            FriendshipStatus = FriendPair.Status.Invited,
            TimeStamp = user.TimeStamp,
        };

        await _context.FriendPairs.AddAsync(friendPair);
        await _context.SaveChangesAsync();
    }

    public async Task AcceptFriendRequest(Guid userId, string friendUsername)
    {
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Username == friendUsername);
        if (friend is null) {
            throw new BadRequestException("Friend request not found");
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
    public async Task RejectFriendRequest(Guid userId, string username)
    {
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (friend is null) {
            throw new BadRequestException("Friend request not found");
        }

        // remove the friend pair or both pairs if they exist
        var friendPair = await _context.FriendPairs
            .FirstOrDefaultAsync(fp => fp.UserId == friend.Id && fp.FriendId == userId);
        if (friendPair is null) {
            throw new BadRequestException("Friend request not found");
        }

        _context.FriendPairs.Remove(friendPair);

        var reversePair = await _context.FriendPairs
            .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.FriendId == friend.Id);
        if (reversePair is not null) {
            _context.FriendPairs.Remove(reversePair);
        }

        await _context.SaveChangesAsync();
    }
    // todo
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

    public async Task<IEnumerable<string>> SearchFriends(Guid userId, string username)
    {
        var friends = await _context.Users
            .Where(u => _context.FriendPairs
                .Where(fp => fp.UserId == userId && fp.FriendshipStatus == FriendPair.Status.Friends)
                .Select(fp => fp.FriendId)
                .Contains(u.Id) && u.Username.ToLower().StartsWith(username))
            .Select(u => u.Username)
            .ToListAsync();

        return friends;
    }

    public async Task<IEnumerable<SearchUserDto>> SearchUsers(Guid userId, string username)
    {
        var users = await _context.Users
            .Where(u => !_context.FriendPairs
                .Any(fp => fp.UserId == userId &&
                           (fp.FriendshipStatus == FriendPair.Status.Friends || fp.FriendshipStatus == FriendPair.Status.Invited) &&
                           fp.FriendId == u.Id) &&
                         EF.Functions.Like(u.Username, username + "%"))
            .Select(u => new SearchUserDto { Username = u.Username, IsInvited = false })
            .ToListAsync();

        return users;
    }

    public async Task UpdatePublicKey(Guid userId, string publicKey)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) {
            throw new BadRequestException("User not found");
        }

        user.PublicKey = publicKey;
        await _context.SaveChangesAsync();
    }
}
