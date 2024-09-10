using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClerkDemo.Database.EntityFramework;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users").HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.FirstName).HasColumnName("first_name");
        builder.Property(d => d.LastName).HasColumnName("last_name");
        builder.Property(d => d.EmailAddress).HasColumnName("email_address");
        builder.Property(d => d.PhoneNumber).HasColumnName("phone_number");
        builder.Property(d => d.ClerkId).HasColumnName("clerk_id");

        builder.Property(d => d.CreatedAt).HasColumnName("created_at");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at");
        builder.Property(d => d.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(d => !d.DeletedAt.HasValue);
    }
}
