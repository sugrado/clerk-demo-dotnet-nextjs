namespace ClerkDemo
{
    public class ClerkOptions
    {
        public const string Clerk = "Clerk";

        public string UsersWebHookSecret { get; set; } = string.Empty;
        public string SessionsWebHookSecret { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string OAuthClientId { get; set; } = string.Empty;
        public string OAuthClientSecret { get; set; } = string.Empty;
    }
}
