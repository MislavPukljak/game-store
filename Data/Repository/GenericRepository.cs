using Data.SQL.Data;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly GameStoreContext _context;

    public GenericRepository(GameStoreContext context)
    {
        _context = context;
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Set<TEntity>().FindAsync(new object[] { id }, CancellationToken.None);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }
}
