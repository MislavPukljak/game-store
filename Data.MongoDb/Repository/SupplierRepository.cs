using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;
using MongoDB.Driver;

namespace Data.MongoDb.Repository;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    private readonly IMongoCollection<Supplier> _dbSet;

    public SupplierRepository(IMongoContext context)
        : base(context, "suppliers")
    {
        _dbSet = context.GetCollection<Supplier>("suppliers");
    }

    public async Task<Supplier> GetByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
    {
        var filter = Builders<Supplier>.Filter.Eq("CompanyName", companyName);

        var suppliers = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return suppliers;
    }

    public async Task<Supplier> GetBySupplierId(int id, CancellationToken cancellationToken)
    {
        var filter = Builders<Supplier>.Filter.Eq("SupplierID", id);

        var suppliers = await _dbSet
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return suppliers;
    }

    public async Task DeleteSupplierAsync(string name, CancellationToken cancellationToken)
    {
        var filter = Builders<Supplier>.Filter.Eq("CompanyName", name);

        await _dbSet.DeleteOneAsync(filter, cancellationToken);
    }
}
