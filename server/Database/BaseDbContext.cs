using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;
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
        // TODO: columns
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").ValueGeneratedOnUpdate();
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at").ValueGeneratedOnUpdate();
    }

    private void SessionConfiguration(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions").HasKey(p => p.Id);

        builder.Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();
        // TODO: columns
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").ValueGeneratedOnUpdate();
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at").ValueGeneratedOnUpdate();
    }
}
