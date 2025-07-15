using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace Data.SQL.Repository;

public class GenreRepository : GenericRepository<Genre>, IGenreRepository
{
    private readonly GameStoreContext _context;

    public GenreRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public override async Task AddAsync(Genre entity, CancellationToken cancellationToken)
    {
        await _context.Genres.AddAsync(entity, cancellationToken);
    }

    public async Task<List<Genre>> GetByIds(List<int> ids, CancellationToken cancellationToken)
    {
        return await _context.Genres
            .Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Genre> GetByCategoryName(string name, CancellationToken cancellationToken)
       => await _context.Genres.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public async Task<IEnumerable<Genre>> GetGenresByGameKey(string key, CancellationToken cancellationToken)
    {
        return await _context.Set<Game>()
            .Where(x => x.Alias == key)
            .SelectMany(x => x.Genres)
            .ToListAsync(cancellationToken);
    }
}
