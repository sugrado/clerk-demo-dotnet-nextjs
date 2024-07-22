namespace ClerkDemo.ConfigurationModels;

public class TokenOptions
{
    public const string Clerk = "Token";

    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string SessionCookieName { get; set; } = string.Empty;
}
