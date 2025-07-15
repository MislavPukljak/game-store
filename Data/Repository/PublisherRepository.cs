using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    private readonly GameStoreContext _context;

    public PublisherRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Publisher> GetPublisherByCompayName(string name, CancellationToken cancellationToken)
       => await _context.Publishers.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyName == name, cancellationToken);

    public async Task<Publisher> GetPublisherByGameKey(string key, CancellationToken cancellationToken)
    {
        return await _context.Set<Game>()
            .Where(x => x.Alias == key)
            .Select(x => x.Publishers)
         .FirstOrDefaultAsync(cancellationToken);
    }
}
