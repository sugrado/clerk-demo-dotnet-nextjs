using ClerkDemo.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using static MongoDB.Driver.IAsyncCursorSourceExtensions;

namespace ClerkDemo.Database.MongoDB;


public abstract class MongoRepository<T> : IMongoRepository<T> where T : Entity
{
    private readonly IMongoCollection<T> _collection;

    protected MongoRepository(MongoConnectionSettings mongoConnectionSetting, string collectionName)
    {
        MongoClient client = new(mongoConnectionSetting.ConnectionString); ;
        IMongoDatabase database = client.GetDatabase(mongoConnectionSetting.DatabaseName);
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<T> queryable = _collection.AsQueryable();
        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }
        return await queryable.ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<T> queryable = _collection.AsQueryable();
        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }
        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> Any(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<T> queryable = _collection.AsQueryable();
        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }
        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(T entity)
    {
        InsertOneOptions options = new() { BypassDocumentValidation = false };
        entity.CreatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity, options);
    }

    public async Task AddManyAsync(IEnumerable<T> entities)
    {
        entities = entities.Select(x =>
        {
            x.CreatedAt = DateTime.UtcNow;
            return x;
        });
        BulkWriteOptions options = new() { IsOrdered = false, BypassDocumentValidation = false };
        await _collection.BulkWriteAsync((IEnumerable<WriteModel<T>>)entities, options);
    }

    public async Task UpdateAsync(Expression<Func<T, bool>> filter, Dictionary<Expression<Func<T, object>>, object> updates)
    {
        UpdateDefinition<T> updateDefinition = Builders<T>
            .Update
            .Combine(updates.Select(x => Builders<T>.Update.Set(x.Key, x.Value)))
            .Set(x => x.UpdatedAt, DateTime.UtcNow);
        await _collection.UpdateOneAsync(filter, updateDefinition);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        T? entity = await GetAsync(p => p.Id == id);
        if (entity is null)
        {
            return;
        }
        UpdateDefinition<T> updateDefinition = Builders<T>.Update.Set(x => x.DeletedAt, DateTime.UtcNow);
        await _collection.UpdateOneAsync(x => x.Id == id, updateDefinition);
    }
}
