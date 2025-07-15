using Data.MongoDb.Entities;

namespace Data.MongoDb.Interfaces;

public interface IShipperRepository : IGenericRepository<Shipper>
{
    Task<Shipper> GetByShipperId(int id, CancellationToken cancellationToken);
}
