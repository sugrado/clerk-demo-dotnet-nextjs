using Clerk.Net.DependencyInjection;
using ClerkDemo.Database;
using ClerkDemo.Database.EntityFramework;
using ClerkDemo.Extensions;
using ClerkDemo.Models;
using ClerkDemo.Services;
using ClerkDemo.Services.Clerk;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<BaseDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.Configure<ClerkOptions>(builder.Configuration.GetSection(ClerkOptions.Clerk));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClerkService>();

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