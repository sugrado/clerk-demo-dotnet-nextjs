using ClerkDemo.Controllers.DTOs;
using ClerkDemo.Database;
using ClerkDemo.Entities;
using System.Linq.Expressions;

namespace ClerkDemo.Services;

public class UserService(UserRepository userRepository, ClerkService clerkService)
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
        User? user = await userRepository.GetAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);
        if (user is not null)
        {
            await userRepository.DeleteAsync(user.Id);
        }
    }

    private async Task HandleUpdatedEvent(ClerkEventData data)
    {
        User? user = await userRepository.GetAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);
        if (user is not null)
        {
            Dictionary<Expression<Func<User, object>>, object> updates = [];

            if (user.Email != data.EmailAddresses![0].Email)
                updates.Add(u => u.Email, data.EmailAddresses![0].Email);

            if (updates.Count > 0)
                await userRepository.UpdateAsync(x => x.Id == user.Id, updates);
        }
    }

    private async Task HandleCreatedEvent(ClerkEventData data)
    {
        User? user = await userRepository.GetAsync(p => p.ClerkId.Equals(data.Id) && !p.DeletedAt.HasValue);

        if (user is null)
        {
            await userRepository.AddAsync(new User
            {
                ClerkId = data.Id,
                Email = data.EmailAddresses![0].Email
            });
        }
    }

    public async Task<List<User>> GetUsers()
    {
        return await userRepository.GetListAsync(p => !p.DeletedAt.HasValue);
    }

    public async Task<List<User>> GetUsersByIds(string[] ids)
    {
        return await userRepository.GetListAsync(p => ids.Contains(p.ClerkId) && !p.DeletedAt.HasValue);
    }

    public async Task<User?> GetUserById(string id)
    {
        return await userRepository.GetAsync(p => p.ClerkId.Equals(id) && !p.DeletedAt.HasValue);
    }

    public async Task SyncUsersWithClerk()
    {
        List<Clerk.Net.Client.Models.User> clerkUsers = await clerkService.GetList();
        List<User> users = await GetUsers();

        foreach (Clerk.Net.Client.Models.User clerkUser in clerkUsers)
        {
            User? user = users.FirstOrDefault(p => p.ClerkId.Equals(clerkUser.Id));
            if (user is not null)
            {
                Dictionary<Expression<Func<User, object>>, object> updates = [];

                if (user.Email != clerkUser.EmailAddresses![0].EmailAddressProp)
                    updates.Add(u => u.Email, clerkUser.EmailAddresses![0].EmailAddressProp!);

                if (updates.Count > 0)
                    await userRepository.UpdateAsync(x => x.Id == user.Id, updates);
            }
            else
            {
                await userRepository.AddAsync(new User
                {
                    ClerkId = clerkUser.Id!,
                    Email = clerkUser.EmailAddresses![0].EmailAddressProp!
                });
            }
        }

        foreach (User user in users)
        {
            Clerk.Net.Client.Models.User? clerkUser = clerkUsers.FirstOrDefault(p => p.Id!.Equals(user.ClerkId));
            if (clerkUser is null)
            {
                await userRepository.UpdateAsync(x => x.Id == user.Id, new()
                {
                    { u => u.DeletedAt!, DateTime.UtcNow }
                });
            }
        }
    }
}