using ClerkDemo.Entities;

namespace ClerkDemo.Database.EntityFramework;

public class UserRepository(BaseDbContext context) : EfRepository<User, Guid>(context), IUserRepository
{
}

