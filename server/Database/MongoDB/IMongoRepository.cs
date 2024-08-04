using ClerkDemo.Entities;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace ClerkDemo.Database.MongoDB;

public interface IMongoRepository<T> where T : Entity
{
    Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    Task<bool> Any(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task AddAsync(T entity);
    Task AddManyAsync(IEnumerable<T> entities);
    Task UpdateAsync(Expression<Func<T, bool>> filter, Dictionary<Expression<Func<T, object>>, object> updates);
    Task DeleteAsync(ObjectId id);
}