namespace Data.SQL.Interfaces;

public interface IGenericRepository<TEntity>
    where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    void Update(TEntity entity);

    void DeleteAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
}
