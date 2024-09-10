namespace ClerkDemo.Entities;

public class User : Entity<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }//ücretli
    public required string ClerkId { get; set; }
}
