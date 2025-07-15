using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class PlatformRepository : GenericRepository<Entities.Platform>, IPlatformRepository
{
    private readonly GameStoreContext _context;

    public PlatformRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<List<Entities.Platform>> GetByIds(List<int> ids, CancellationToken cancellationToken)
    {
        return await _context.Platforms
            .Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Entities.Platform>> GetPlatformsByGameKey(string key, CancellationToken cancellationToken)
    {
        return await _context.Set<Game>()
            .Where(x => x.Alias == key)
            .SelectMany(x => x.Platforms)
         .ToListAsync(cancellationToken);
    }

    public override async Task<Entities.Platform> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Set<Entities.Platform>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Entities.Platform>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<Entities.Platform>()
            .Include(g => g.Games)
            .ToListAsync(cancellationToken);
    }
}
