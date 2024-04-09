﻿using ChatterAPI.Models;
using ChatterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatterAPI.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ChattingController : ControllerBase
{
    private readonly ChattingService _service;

    public ChattingController(ChattingService service)
    {
        _service = service;
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages()
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var messages = await _service.GetAwaitingMessages(userId);

        return Ok(messages);
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        await _service.SendMessage(userId, message);

        return Ok();
    }

}