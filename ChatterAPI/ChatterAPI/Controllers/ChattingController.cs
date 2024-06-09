using ChatterAPI.Models;
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
        var messages = await _service.GetAwaitingMessages(UserId);


        if (messages.Count() > 0) {
            Console.WriteLine("Messages received");
        }

        return Ok(messages);
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
    {
        await _service.SendMessage(UserId, Username, message);

        if (message.Type == Entities.Message.MessageType.Image) {
            Console.WriteLine("Image message sent");
        }

        return Ok();
    }

    private Guid UserId => Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    private string Username => User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
}
