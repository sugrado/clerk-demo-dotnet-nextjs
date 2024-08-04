using MongoDB.Driver;

namespace ClerkDemo.Database.MongoDB;
public class MongoConnectionSettings
{
    public const string MongoSettings = "MongoSettings";

    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}