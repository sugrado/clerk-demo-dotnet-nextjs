using ClerkDemo.Entities;

namespace ClerkDemo.Database;

public interface IUserRepository : IRepository<User, Guid>
{
}

