namespace ClerkDemo.ConfigurationModels;
public class MongoConnectionSettings
{
    public const string MongoSettings = "MongoSettings";

    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}