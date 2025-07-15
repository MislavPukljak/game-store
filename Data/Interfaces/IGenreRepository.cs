using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface IGenreRepository : IGenericRepository<Genre>
{
    Task<List<Genre>> GetByIds(List<int> ids, CancellationToken cancellationToken);

    Task<IEnumerable<Genre>> GetGenresByGameKey(string key, CancellationToken cancellationToken);

    Task<Genre> GetByCategoryName(string name, CancellationToken cancellationToken);
}