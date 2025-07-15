using Data.MongoDb.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.MongoDb.Repository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly IMongoCollection<TEntity> _dbSet;

    public GenericRepository(IMongoContext context, string collectionName)
    {
        _dbSet = context.GetCollection<TEntity>(collectionName);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TEntity>.Filter.Eq("_id", objectId);

        await _dbSet.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<TEntity>.Filter.Empty;
        var all = await _dbSet.FindAsync(filter, cancellationToken: cancellationToken);

        return all.ToList(cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TEntity>.Filter.Eq("_id", objectId);

        var entity = await _dbSet.FindAsync(filter, cancellationToken: cancellationToken);

        return entity.SingleOrDefault(cancellationToken);
    }

    public async Task Update(string id, TEntity entity, CancellationToken cancellationToken)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TEntity>.Filter.Eq("_id", objectId);

        await _dbSet.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }
}
