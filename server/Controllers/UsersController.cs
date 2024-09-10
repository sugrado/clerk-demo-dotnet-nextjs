using ClerkDemo.Models;
using ClerkDemo.Services;
using ClerkDemo.Services.Clerk.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Svix;
using Svix.Exceptions;
using System.Net;

namespace ClerkDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService, IOptions<ClerkOptions> clerkOptions, ILogger<UsersController> logger) : ControllerBase
{
    [HttpPost]
    public async Task HandleWebhookEvents([FromBody] object body)
    {
        ValidateWebHook();
        UserEvent clerkEvent = JsonConvert.DeserializeObject<UserEvent>(body.ToString()!)!;
        await userService.HandleEvent(clerkEvent);
    }

    [HttpGet("me"), Authorize]
    public IActionResult GetMyInfo()
    {
        return Ok(new { Name = "Ali", Age = 23 });
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> Get()
    {
        return Ok(await userService.GetUsers());
    }

    [HttpGet("{id}"), Authorize]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        return Ok(await userService.GetUserById(id));
    }

    [HttpPost("sync")]
    public async Task Sync()
    {
        await userService.SyncWithClerk();
    }

    private void ValidateWebHook()
    {
        Request.Headers.TryGetValue("svix-id", out StringValues svixId);
        Request.Headers.TryGetValue("svix-timestamp", out StringValues svixTimestamp);
        Request.Headers.TryGetValue("svix-signature", out StringValues svixSignature);

        WebHeaderCollection headers = [];
        headers.Set("svix-id", svixId);
        headers.Set("svix-timestamp", svixTimestamp);
        headers.Set("svix-signature", svixSignature);

        string? payload = Request.Body.ToString();
        try
        {
            Webhook webHook = new(clerkOptions.Value.UsersWebHookSecret);
            webHook.Verify(payload, headers);
        }
        catch (WebhookVerificationException ex)
        {
            logger.LogError(ex, "Failed to verify webhook");
        }
    }
}
