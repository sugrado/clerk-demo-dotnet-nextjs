using ClerkDemo.Database;
using ClerkDemo.Entities;
using ClerkDemo.Services.Clerk;
using ClerkDemo.Services.Clerk.Events;
using System.Linq.Expressions;

namespace ClerkDemo.Services;

public class UserService(IUserRepository userRepository, ClerkService clerkService)
{
    public async Task HandleEvent(UserEvent clerkEvent)
    {
        switch (clerkEvent.Type)
        {
            case "user.deleted":
                await HandleDeletedEvent(clerkEvent.Data);
                break;
            case "user.updated":
                await HandleUpdatedEvent(clerkEvent.Data!);
                break;
            case "user.created":
                await HandleCreatedEvent(clerkEvent.Data!);
                break;
        }
    }

    private async Task HandleDeletedEvent(UserEventData data)
    {
        User? user = await userRepository.GetAsync(p => p.ClerkId.Equals(data.Id));
        if (user is not null)
        {
            await userRepository.DeleteAsync(user);
        }
    }

    private async Task HandleUpdatedEvent(UserEventData data)
    {
        User? user = await userRepository.GetAsync(p => p.ClerkId.Equals(data.Id));
        if (user is not null)
        {
            List<Expression<Func<User, object>>> updatedProperties = [];

            if (user.FirstName != data.FirstName)
            {
                user.FirstName = data.FirstName;
                updatedProperties.Add(u => u.FirstName);
            }

            if (user.LastName != data.LastName)
            {
                user.LastName = data.LastName;
                updatedProperties.Add(u => u.LastName);
            }

            var primaryEmail = data.EmailAddresses!.FirstOrDefault(p => p.Id.Equals(data.PrimaryEmailAddressId));
            if (primaryEmail is not null && user.EmailAddress != primaryEmail.Email)
            {
                user.EmailAddress = primaryEmail.Email;
                updatedProperties.Add(u => u.EmailAddress);
            }

            if (updatedProperties.Count > 0)
                await userRepository.UpdateAsync(user, [.. updatedProperties]);
        }
    }

    private async Task HandleCreatedEvent(UserEventData data)
    {
        User? user = await userRepository.GetAsync(p => p.ClerkId.Equals(data.Id));

        if (user is null)
        {
            await userRepository.AddAsync(new User
            {
                ClerkId = data.Id,
                EmailAddress = data.EmailAddresses!.FirstOrDefault(p => p.Id.Equals(data.PrimaryEmailAddressId))!.Email,
                FirstName = data.FirstName,
                LastName = data.LastName,
            });
        }
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await userRepository.GetListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByIds(string[] ids)
    {
        return await userRepository.GetListAsync(p => ids.Contains(p.ClerkId));
    }

    public async Task<User?> GetUserById(string id)
    {
        return await userRepository.GetAsync(p => p.ClerkId.Equals(id));
    }

    public async Task SyncWithClerk()
    {
        var clerkUsers = await clerkService.GetList();
        IEnumerable<User> users = await GetUsers();

        foreach (var clerkUser in clerkUsers)
        {
            User? user = users.FirstOrDefault(p => p.ClerkId.Equals(clerkUser.Id));
            if (user is not null)
            {
                List<Expression<Func<User, object>>> updatedProperties = [];

                if (user.FirstName != clerkUser.FirstName)
                {
                    user.FirstName = clerkUser.FirstName!;
                    updatedProperties.Add(u => u.FirstName);
                }

                if (user.LastName != clerkUser.LastName)
                {
                    user.LastName = clerkUser.LastName!;
                    updatedProperties.Add(u => u.LastName);
                }

                var primaryEmail = clerkUser.EmailAddresses!.FirstOrDefault(p => p.Id!.Equals(clerkUser.PrimaryEmailAddressId));
                if (primaryEmail is not null && primaryEmail.EmailAddressProp is not null && user.EmailAddress != primaryEmail.EmailAddressProp)
                {
                    user.EmailAddress = primaryEmail.EmailAddressProp;
                    updatedProperties.Add(u => u.EmailAddress);
                }

                if (updatedProperties.Count > 0)
                    await userRepository.UpdateAsync(user, [.. updatedProperties]);
            }
            else
            {
                await userRepository.AddAsync(new User
                {
                    ClerkId = clerkUser.Id!,
                    EmailAddress = clerkUser.EmailAddresses!.FirstOrDefault(p => p.Id!.Equals(clerkUser.PrimaryEmailAddressId))!.EmailAddressProp!,
                    FirstName = clerkUser.FirstName!,
                    LastName = clerkUser.LastName!,
                });
            }
        }

        foreach (User user in users)
        {
            var clerkUser = clerkUsers.FirstOrDefault(p => p.Id!.Equals(user.ClerkId));
            if (clerkUser is null)
            {
                await userRepository.DeleteAsync(user);
            }
        }
    }
}