using ClerkDemo;
using ClerkDemo.Database;
using ClerkDemo.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<BaseDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

ClerkOptions clerkOptions = builder.Configuration.GetOptions<ClerkOptions>("Clerk");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "Clerk";
    })
    .AddCookie()
    .AddOAuth("Clerk", options =>
    {
        options.ClientId = clerkOptions.OAuthClientId;
        options.ClientSecret = clerkOptions.OAuthClientSecret;
        options.AuthorizationEndpoint = $"{clerkOptions.Domain}/oauth/authorize";
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.CallbackPath = new PathString("/oauth/callback");
        options.TokenEndpoint = $"{clerkOptions.Domain}/oauth/token";
        options.UserInformationEndpoint = $"{clerkOptions.Domain}/oauth/userinfo";
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                HttpRequestMessage? request = new(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                HttpResponseMessage? response = await context.Backchannel.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();
                string? json = await response.Content.ReadAsStringAsync();
                context.RunClaimActions(JsonDocument.Parse(json).RootElement);
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
WebAPIOptions webApiConfiguration = builder.Configuration.GetOptions<WebAPIOptions>("WebAPI");
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
    {
        p.WithOrigins(webApiConfiguration.AllowedOrigins)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
    })
);
builder.Services.AddSwaggerGen();
builder.Services.Configure<ClerkOptions>(builder.Configuration.GetSection(ClerkOptions.Clerk));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();