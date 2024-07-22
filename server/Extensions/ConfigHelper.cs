namespace ClerkDemo.Extensions;

public static class ConfigHelper
{
    public static IConfiguration? Config;
    public static void Initialize(IConfiguration configuration)
    {
        Config = configuration;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => configuration.GetSection(sectionName).Get<T>()
            ?? throw new InvalidOperationException($"{sectionName} section cannot found in configuration.");
}
