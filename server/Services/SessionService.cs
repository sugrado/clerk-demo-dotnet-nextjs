using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Database;
using ClerkDemo.Entities;

namespace ClerkDemo.Services;

public class SessionService(SessionRepository sessionRepository, UserService userService)
{
    public async Task HandleEvent(ClerkEvent clerkEvent)
    {
        switch (clerkEvent.Type)
        {
            case "session.removed":
                await HandleRemovedEvent(clerkEvent.Data!);
                break;
            case "session.ended":
                await HandleEndedEvent(clerkEvent.Data!);
                break;
            case "session.revoked":
                await HandleRevokedEvent(clerkEvent.Data!);
                break;
            case "session.created":
                await HandleCreatedEvent(clerkEvent.Data!);
                break;
        }
    }

    private async Task HandleCreatedEvent(ClerkEventData data)
    {
        User? user = await userService.GetUserById(data.UserId!);

        if (user is not null)
        {
            Session session = new()
            {
                ClerkId = data.Id,
                UserId = user.Id,
                ExpireAt = new DateTime(long.Parse(data.ExpireAt!)),
            };
            await sessionRepository.AddAsync(session);
        }
    }

    private async Task HandleRevokedEvent(ClerkEventData data)
    {
        Session? session = await sessionRepository.GetAsync(p => p.ClerkId.Equals(data.Id) && !p.RevokedAt.HasValue && !p.DeletedAt.HasValue);
        if (session is not null)
        {
            await sessionRepository.UpdateAsync(x => x.Id == session.Id, new()
            {
                { s => s.RevokedAt!, DateTime.UtcNow }
            });
        }
    }

    private async Task HandleRemovedEvent(ClerkEventData data)
    {
        Session? session = await sessionRepository.GetAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);
        if (session is not null)
        {
            await sessionRepository.DeleteAsync(session.Id);
        }
    }

    private async Task HandleEndedEvent(ClerkEventData data)
    {
        Session? session = await sessionRepository.GetAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue && !p.EndedAt.HasValue);
        if (session is not null)
        {
            await sessionRepository.UpdateAsync(x => x.Id == session.Id, new()
            {
                { s => s.EndedAt!, DateTime.UtcNow }
            });
        }
    }
}
