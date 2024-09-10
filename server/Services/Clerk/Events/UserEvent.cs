using Newtonsoft.Json;

namespace ClerkDemo.Services.Clerk.Events;

public record UserEventData(
    [JsonProperty("created_at")] long CreatedAt,
    [JsonProperty("email_addresses")] IReadOnlyList<EmailAddress> EmailAddresses,
    [JsonProperty("first_name")] string FirstName,
    [JsonProperty("id")] string Id,
    [JsonProperty("last_name")] string LastName,
    [JsonProperty("primary_email_address_id")] string PrimaryEmailAddressId,
    [JsonProperty("updated_at")] long UpdatedAt
);

public record EmailAddress(
    [JsonProperty("email_address")] string Email,
    [JsonProperty("id")] string Id
);

public record UserEvent(
    [JsonProperty("data")] UserEventData Data,
    [JsonProperty("type")] string Type
);
