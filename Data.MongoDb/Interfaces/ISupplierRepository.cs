using Data.MongoDb.Entities;

namespace Data.MongoDb.Interfaces;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<Supplier> GetByCompanyNameAsync(string companyName, CancellationToken cancellationToken);

    Task<Supplier> GetBySupplierId(int id, CancellationToken cancellationToken);

    Task DeleteSupplierAsync(string name, CancellationToken cancellationToken);
}
