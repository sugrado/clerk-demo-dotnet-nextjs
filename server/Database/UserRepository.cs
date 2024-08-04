using ClerkDemo.ConfigurationModels;
using ClerkDemo.Database.MongoDB;
using ClerkDemo.Entities;
using Microsoft.Extensions.Options;

namespace ClerkDemo.Database;

public class UserRepository(IOptions<MongoConnectionSettings> mongoConnectionSetting) : MongoRepository<User>(mongoConnectionSetting.Value, "users")
{
}

