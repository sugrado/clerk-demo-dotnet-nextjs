using Clerk.Net.DependencyInjection;
using ClerkDemo.ConfigurationModels;
using ClerkDemo.Database;
using ClerkDemo.Extensions;
using ClerkDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

ConfigHelper.Initialize(builder.Configuration);

builder.Services.AddJwt(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddClerkApiClient(config =>
{
    config.SecretKey = builder.Configuration["Clerk:SecretKey"]!;
});

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
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ClerkService>();

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