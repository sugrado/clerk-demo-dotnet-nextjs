using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Svix;
using Svix.Exceptions;
using System.Net;

namespace ClerkDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IOptions<ClerkOptions> clerkOptions, ILogger<UsersController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task HandleEvents()
        {
            var payload = Request.Body.ToString();

            Request.Headers.TryGetValue("svix-id", out var svixId);
            Request.Headers.TryGetValue("svix-timestamp", out var svixTimestamp);
            Request.Headers.TryGetValue("svix-signature", out var svixSignature);
            var headers = new WebHeaderCollection();
            headers.Set("svix-id", svixId);
            headers.Set("svix-timestamp", svixTimestamp);
            headers.Set("svix-signature", svixSignature);


            Webhook webHook = new(clerkOptions.Value.UsersWebHookSecret);


            try
            {
                webHook.Verify(payload, headers);
            }
            catch (WebhookVerificationException ex)
            {
                logger.LogError(ex, "Failed to verify webhook");
            }

            Console.WriteLine(Request.Body);
            await Task.CompletedTask;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMyInfo()
        {
            return Ok(new { Name = "Ali", Age = 23 });
        }
    }
}
