using ClerkDemo.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ClerkDemo.Extensions;

public static class JwtConfiguration
{
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        TokenOptions tokenOptions = configuration.GetOptions<TokenOptions>(TokenOptions.Token);
        ClerkOptions clerkOptions = configuration.GetOptions<ClerkOptions>(ClerkOptions.Clerk);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (_, _, _, _) =>
                {
                    HttpClient httpClient = new();
                    string keySet = httpClient.GetStringAsync(clerkOptions.JWKSUrl).Result;
                    return new JsonWebKeySet(keySet).GetSigningKeys();
                },
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (string.IsNullOrWhiteSpace(context.Token)
                        && !string.IsNullOrWhiteSpace(context.Request.Cookies[tokenOptions.SessionCookieName]))
                        context.Token = context.Request.Cookies[tokenOptions.SessionCookieName];
                    return Task.CompletedTask;
                }
            };
        });
    }
}
