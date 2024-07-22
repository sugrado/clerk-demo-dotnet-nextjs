namespace ClerkDemo.ConfigurationModels;

public class WebAPIOptions
{
    public const string Clerk = "WebAPI";

    public string[] AllowedOrigins { get; set; } = [];
}
