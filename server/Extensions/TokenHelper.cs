using ClerkDemo.ConfigurationModels;
using Microsoft.IdentityModel.Tokens;

namespace ClerkDemo.Extensions;

public static class TokenHelper
{
    public static IList<SecurityKey> SigningKeyResolver(
        string token,
        SecurityToken securityToken,
        string kid,
        TokenValidationParameters tokenValidationParameters)
    {
        ClerkOptions clerkOptions = ConfigHelper.Config!.GetOptions<ClerkOptions>(ClerkOptions.Clerk);
        HttpClient client = new();
        string keySet = client.GetStringAsync(clerkOptions.JWKSUrl).Result;
        return new JsonWebKeySet(keySet).GetSigningKeys();
    }
}
