using Newtonsoft.Json;

namespace ClerkDemo.Controllers.DTOs;

public class ClerkEvent
{
    [JsonProperty("data")]
    public ClerkEventData? Data { get; set; }

    [JsonProperty("object")]
    public required string Object { get; set; }

    [JsonProperty("type")]
    public required string Type { get; set; }
}

public class ClerkEventData
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("user_id")]
    public string? UserId { get; set; }

    [JsonProperty("created_at")]
    public string? CreatedAt { get; set; }

    [JsonProperty("expire_at")]
    public string? ExpireAt { get; set; }

    [JsonProperty("updated_at")]
    public string? UpdatedAt { get; set; }

    [JsonProperty("email_addresses")]
    public List<ClerkEventDataEmailAddress>? EmailAddresses { get; set; }
}

public class ClerkEventDataEmailAddress
{

    [JsonProperty("email_address")]
    public required string Email { get; set; }
}