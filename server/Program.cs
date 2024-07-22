using ClerkDemo.ConfigurationModels;
using ClerkDemo.Database;
using ClerkDemo.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<BaseDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

ConfigHelper.Initialize(builder.Configuration);

builder.Services.AddJwt(builder.Configuration);

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