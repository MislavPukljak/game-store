using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface IPublisherRepository : IGenericRepository<Publisher>
{
    Task<Publisher> GetPublisherByCompayName(string name, CancellationToken cancellationToken);

    Task<Publisher> GetPublisherByGameKey(string key, CancellationToken cancellationToken);
}
