using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;
using MongoDB.Driver;

namespace Data.MongoDb.Repository;

public class ShipperRepository : GenericRepository<Shipper>, IShipperRepository
{
    private readonly IMongoCollection<Shipper> _dbSet;

    public ShipperRepository(IMongoContext context)
        : base(context, "shippers")
    {
        _dbSet = context.GetCollection<Shipper>("shippers");
    }

    public async Task<Shipper> GetByShipperId(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<Shipper>.Filter.Eq("ShipperID", id);

        var shippers = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return shippers;
    }
}
