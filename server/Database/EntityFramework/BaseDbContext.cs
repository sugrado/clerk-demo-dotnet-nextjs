using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace ClerkDemo.Database.EntityFramework;

public partial class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
