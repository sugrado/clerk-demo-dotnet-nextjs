using ClerkDemo.Entities;
using System.Linq.Expressions;

namespace ClerkDemo.Database;

public interface IRepository<TEntity, TEntityId> where TEntity : Entity<TEntityId>
{
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties);

    Task<TEntity> DeleteAsync(TEntity entity);
}
