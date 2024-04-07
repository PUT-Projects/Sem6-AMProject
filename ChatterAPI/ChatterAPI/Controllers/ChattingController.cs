using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatterAPI.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ChattingController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        return Ok();
    }
}
