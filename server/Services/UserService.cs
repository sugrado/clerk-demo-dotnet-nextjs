using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Database;
using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClerkDemo.Services;

public class UserService(BaseDbContext context)
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
}
