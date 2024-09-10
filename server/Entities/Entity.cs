namespace ClerkDemo.Entities;

public class Entity<TId>
{
    public TId Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Entity()
    {
        Id = default!;
    }

    public Entity(TId id)
    {
        Id = id;
    }
}