using ClerkDemo.ConfigurationModels;
using ClerkDemo.Database.MongoDB;
using ClerkDemo.Entities;
using Microsoft.Extensions.Options;

namespace ClerkDemo.Database;

public class SessionRepository(IOptions<MongoConnectionSettings> mongoConnectionSetting) : MongoRepository<Session>(mongoConnectionSetting.Value, "sessions")
{
}

