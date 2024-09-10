using ClerkDemo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace ClerkDemo.Database.EntityFramework;

public class EfRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId> where TEntity : Entity<TEntityId>
{
    protected readonly BaseDbContext Context;

    public EfRepository(BaseDbContext context)
    {
        Context = context;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await Context.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Context.Set<TEntity>();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Context.Set<TEntity>();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        EntityEntry<TEntity> attachedEntity = Context.Attach(entity);
        foreach (var property in updatedProperties)
        {
            attachedEntity.Property(property).IsModified = true;
        }
        if (updatedProperties.Length > 0)
            await Context.SaveChangesAsync();
        return entity;
    }
}
