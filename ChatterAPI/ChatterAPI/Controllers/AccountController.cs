using ChatterAPI.Entities;
using ChatterAPI.Models;
using ChatterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountService _service;

    public AccountController(AccountService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        await _service.RegisterUser(dto);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _service.GenerateJWT(dto);

        return Ok(token);
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetUser()
    {
        var username = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

        return Ok(new { UserId, username });
    }

    [Authorize]
    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var friends = await _service.GetFriends(UserId);

        return Ok(friends);
    }

    [Authorize]
    [HttpPost("friends/invite")]
    public async Task<IActionResult> InviteFriend([FromBody] string username)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        await _service.AddFriend(userId, username);

        return Ok();
    }

    [Authorize]
    [HttpPost("friends/accept")]
    public async Task<IActionResult> AcceptFriend([FromBody] string username)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        await _service.AcceptFriendRequest(userId, username);

        return Ok();
    }

    [Authorize]
    [HttpPost("friends/reject")]
    public async Task<IActionResult> RejectFriend([FromBody] string username)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        await _service.RejectFriendRequest(userId, username);

        return Ok();
    }

    [Authorize]
    [HttpPost("friends/search")]
    public async Task<IActionResult> SearchFriends([FromBody] string username)
    {
       var users = await _service.SearchFriends(UserId, username);

        return Ok(users);
    }

    [Authorize]
    [HttpPost("users/search")]
    public async Task<IActionResult> SearchUsers([FromBody] string username)
    {
        var users = await _service.SearchUsers(UserId, username);

        return Ok(users);
    }

    [Authorize]
    [HttpGet("friends/requests")]
    public async Task<IActionResult> GetFriendRequests()
    {
        var requests = await _service.GetFriendRequests(UserId);

        return Ok(requests);
    }

    private Guid UserId => Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
}
