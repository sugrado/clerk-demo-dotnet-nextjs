using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClerkDemo.Database;

public class BaseDbContext(DbContextOptions<BaseDbContext> options) : DbContext(options)
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        UserConfiguration(modelBuilder.Entity<User>());
        SessionConfiguration(modelBuilder.Entity<Session>());
    }

    private void UserConfiguration(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users").HasKey(p => p.Id);

        builder.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(u => u.ClerkId).HasColumnName("clerk_id").IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").IsRequired();

        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired().ValueGeneratedOnAdd();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired(false).ValueGeneratedOnUpdate();
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at").IsRequired(false).ValueGeneratedOnUpdate();

        builder.HasMany(u => u.Sessions).WithOne(s => s.User).HasForeignKey(s => s.UserId);
    }

    private void SessionConfiguration(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions").HasKey(p => p.Id);

        builder.Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(u => u.ClerkId).HasColumnName("clerk_id").IsRequired();
        builder.Property(u => u.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(u => u.ExpireAt).HasColumnName("expire_at").IsRequired();
        builder.Property(u => u.RevokedAt).HasColumnName("revoked_at").IsRequired(false);
        builder.Property(u => u.EndedAt).HasColumnName("ended_at").IsRequired(false);

        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired().ValueGeneratedOnAdd();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired(false).ValueGeneratedOnUpdate();
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at").IsRequired(false).ValueGeneratedOnUpdate();

        builder.HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.UserId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        foreach (EntityEntry entry in base.ChangeTracker.Entries().Where(a => a.Entity is Entity))
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((Entity)entry.Entity).CreatedAt = now;
                    break;

                case EntityState.Modified:
                    ((Entity)entry.Entity).UpdatedAt = now;
                    break;

                case EntityState.Deleted:
                    ((Entity)entry.Entity).UpdatedAt = now;
                    entry.State = EntityState.Modified;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
