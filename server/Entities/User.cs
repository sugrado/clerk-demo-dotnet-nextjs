namespace ClerkDemo.Entities;

public class User : Entity
{
    public required string ClerkId { get; set; }
    public required string Email { get; set; }
    public ICollection<Session>? Sessions { get; set; }
}
