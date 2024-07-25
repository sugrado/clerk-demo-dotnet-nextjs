using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Database;
using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClerkDemo.Services;

public class UserService(BaseDbContext context, ClerkService clerkService)
{
    public async Task HandleEvent(ClerkEvent clerkEvent)
    {
        switch (clerkEvent.Type)
        {
            case "user.deleted":
                await HandleDeletedEvent(clerkEvent.Data!);
                break;
            case "user.updated":
                await HandleUpdatedEvent(clerkEvent.Data!);
                break;
            case "user.created":
                await HandleCreatedEvent(clerkEvent.Data!);
                break;
        }
    }

    private async Task HandleDeletedEvent(ClerkEventData data)
    {
        User? user = await context.Users.FirstOrDefaultAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);
        if (user is not null)
        {
            user.DeletedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    private async Task HandleUpdatedEvent(ClerkEventData data)
    {
        User? user = await context.Users.FirstOrDefaultAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);
        if (user is not null)
        {
            if (user.Email != data.EmailAddresses![0].Email)
                user.Email = data.EmailAddresses![0].Email;
            await context.SaveChangesAsync();
        }
    }

    private async Task HandleCreatedEvent(ClerkEventData data)
    {
        User? user = await context.Users.FirstOrDefaultAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);
        if (user is null)
        {
            context.Users.Add(new User
            {
                ClerkId = data.Id,
                Email = data.EmailAddresses![0].Email
            });
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetUsers()
    {
        List<Clerk.Net.Client.Models.User> clerkUsers = await clerkService.GetList();
        return clerkUsers
            .Select(p => new User
            {
                ClerkId = p.Id!,
                Email = p.EmailAddresses!.FirstOrDefault()!.EmailAddressProp!
            })
            .ToList();
    }

    public async Task<List<User>> GetUsersByIds(string[] ids)
    {
        List<Clerk.Net.Client.Models.User> clerkUsers = await clerkService.GetList(ids);
        return clerkUsers
            .Select(p => new User
            {
                ClerkId = p.Id!,
                Email = p.EmailAddresses!.FirstOrDefault()!.EmailAddressProp!
            })
            .ToList();
    }

    public async Task<User?> GetUserById(string id)
    {
        Clerk.Net.Client.Models.User? clerkUser = await clerkService.Get(id);
        if (clerkUser is null)
            return null;
        return new User
        {
            ClerkId = clerkUser.Id!,
            Email = clerkUser.EmailAddresses!.FirstOrDefault()!.EmailAddressProp!
        };
    }
}
