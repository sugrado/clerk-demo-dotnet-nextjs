using MongoDB.Bson;

namespace ClerkDemo.Entities;

public class Entity
{
    public ObjectId Id { get; set; }
    public string ObjectId => Id.ToString();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}