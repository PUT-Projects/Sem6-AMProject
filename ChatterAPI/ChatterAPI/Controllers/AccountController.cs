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
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var username = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

        return Ok(new { userId, username });
    }

    [Authorize]
    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var friends = await _service.GetFriends(userId);

        return Ok(friends);
    }

    [Authorize]
    [HttpPost("friends/invite")]
    public async Task<IActionResult> InviteFriend([FromQuery] string u)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        await _service.AddFriend(userId, u);

        return Ok();
    }

    [Authorize]
    [HttpPost("friends/accept")]
    public async Task<IActionResult> AcceptFriend([FromQuery] string u)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        await _service.AcceptFriendRequest(userId, u);

        return Ok();
    }



}
