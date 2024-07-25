using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClerkDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService, ILogger<UsersController> logger) : BaseController(logger)
{
    [HttpPost]
    public async Task HandleWebhookEvents([FromBody] object body)
    {
        ValidateWebhook(body);
        ClerkEvent clerkEvent = JsonConvert.DeserializeObject<ClerkEvent>(body.ToString()!)!;
        await userService.HandleEvent(clerkEvent);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMyInfo()
    {
        return Ok(new { Name = "Ali", Age = 23 });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        return Ok(await userService.GetUsers());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        return Ok(await userService.GetUserById(id));
    }
}
