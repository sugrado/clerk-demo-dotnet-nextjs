using Clerk.Net.Client;
using Clerk.Net.Client.Models;
using Microsoft.Kiota.Abstractions;
using static Clerk.Net.Client.Users.UsersRequestBuilder;

namespace ClerkDemo.Services;

public class ClerkService(ClerkApiClient clerkApiClient)
{
    public async Task<List<User>> GetList()
    {
        List<User>? users = await clerkApiClient.Users.GetAsync();
        return users is null ? [] : users;
    }

    public async Task<List<User>> GetList(string[] ids)
    {
        Action<RequestConfiguration<UsersRequestBuilderGetQueryParameters>> action = new(p =>
        {
            p.QueryParameters.UserId = ids;
        });
        List<User>? users = await clerkApiClient.Users.GetAsync(action);
        return users is null ? [] : users;
    }

    public async Task<User?> Get(string id)
    {
        Action<RequestConfiguration<UsersRequestBuilderGetQueryParameters>> action = new(p =>
        {
            p.QueryParameters.UserId = [id];
        });
        List<User>? users = await clerkApiClient.Users.GetAsync(action);
        return users?.FirstOrDefault();
    }
}
