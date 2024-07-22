using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Database;

namespace ClerkDemo.Services;

public class SessionService(BaseDbContext context)
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
        throw new NotImplementedException();
    }

    private async Task HandleUpdatedEvent(ClerkEventData data)
    {
        throw new NotImplementedException();
    }

    private async Task HandleCreatedEvent(ClerkEventData data)
    {
        throw new NotImplementedException();
    }
}
