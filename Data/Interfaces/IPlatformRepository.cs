namespace Data.SQL.Interfaces;

public interface IPlatformRepository : IGenericRepository<Entities.Platform>
{
    Task<List<Entities.Platform>> GetByIds(List<int> ids, CancellationToken cancellationToken);

    Task<IEnumerable<Entities.Platform>> GetPlatformsByGameKey(string key, CancellationToken cancellationToken);
}
