using ClerkDemo.ConfigurationModels;
using ClerkDemo.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Svix;
using Svix.Exceptions;
using System.Net;

namespace ClerkDemo.Controllers;

public class BaseController(ILogger<UsersController> logger) : ControllerBase
{
    protected void ValidateWebhook(object body)
    {
        ClerkOptions clerkOptions = ConfigHelper.Config!.GetOptions<ClerkOptions>(ClerkOptions.Clerk);

        Request.Headers.TryGetValue("svix-id", out StringValues svixId);
        Request.Headers.TryGetValue("svix-timestamp", out StringValues svixTimestamp);
        Request.Headers.TryGetValue("svix-signature", out StringValues svixSignature);

        var headers = new WebHeaderCollection();
        headers.Set("svix-id", svixId);
        headers.Set("svix-timestamp", svixTimestamp);
        headers.Set("svix-signature", svixSignature);

        Webhook webHook = new(clerkOptions.UsersWebHookSecret);
        string? payload = body.ToString();
        try
        {
            webHook.Verify(payload, headers);
        }
        catch (WebhookVerificationException ex)
        {
            logger.LogError(ex, "Failed to verify webhook");
        }
    }
}
