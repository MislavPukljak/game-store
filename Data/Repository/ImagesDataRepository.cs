using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class ImagesDataRepository : GenericRepository<ImagesData>, IImagesDataRepository
{
    private readonly GameStoreContext _context;

    public ImagesDataRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<ImagesData> GetByNameAsync(string name)
    {
        return await _context.Set<ImagesData>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name);
    }
}
