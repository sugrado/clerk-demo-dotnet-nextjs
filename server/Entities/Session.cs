using MongoDB.Bson;

namespace ClerkDemo.Entities;

public class Session : Entity
{
    public required string ClerkId { get; set; }
    public ObjectId UserId { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
