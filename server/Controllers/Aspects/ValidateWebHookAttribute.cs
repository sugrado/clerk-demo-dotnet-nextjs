using ClerkDemo.ConfigurationModels;
using ClerkDemo.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Svix;
using Svix.Exceptions;
using System.Net;

namespace ClerkDemo.Controllers.Aspects;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateWebHookAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ClerkOptions clerkOptions = ConfigHelper.Config!.GetOptions<ClerkOptions>(ClerkOptions.Clerk);

        context.HttpContext.Request.Headers.TryGetValue("svix-id", out StringValues svixId);
        context.HttpContext.Request.Headers.TryGetValue("svix-timestamp", out StringValues svixTimestamp);
        context.HttpContext.Request.Headers.TryGetValue("svix-signature", out StringValues svixSignature);

        var headers = new WebHeaderCollection();
        headers.Set("svix-id", svixId);
        headers.Set("svix-timestamp", svixTimestamp);
        headers.Set("svix-signature", svixSignature);

        Webhook webHook = new(clerkOptions.UsersWebHookSecret);
        string? payload = context.HttpContext.Request.Body.ToString();
        try
        {
            webHook.Verify(payload, headers);
        }
        catch (WebhookVerificationException ex)
        {
            ILogger<ValidateWebHookAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ValidateWebHookAttribute>>();
            logger.LogError(ex, "Failed to verify webhook");
        }
        base.OnActionExecuting(context);
    }
}
