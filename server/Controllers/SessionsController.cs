using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClerkDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController(SessionService sessionService, ILogger<UsersController> logger) : BaseController(logger)
{
    [HttpPost]
    public async Task HandleWebhookEvents([FromBody] object body)
    {
        ValidateWebhook(body);
        ClerkEvent clerkEvent = JsonConvert.DeserializeObject<ClerkEvent>(body.ToString()!)!;
        await sessionService.HandleEvent(clerkEvent);
    }
}
