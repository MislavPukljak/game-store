using Data.MongoDb.Entities;

namespace Data.MongoDb.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product> GetByProductId(int id, CancellationToken cancellationToken);

    Task<Product> GetByAliasAsync(string alias, CancellationToken cancellationToken);

    Task DeleteProductAsync(string alias, CancellationToken cancellationToken);

    Task<IEnumerable<Product>> GetProductBySuplierId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Product>> GetProductByCategoryId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Product>> GetProductBySupplier(string companyName, CancellationToken cancellationToken);

    Task<ProductPageInfo> GetProductsAsync(ProductFilter filter, CancellationToken cancellationToken);

    Task UpdateUnitInStock(string alias, Product product, CancellationToken cancellationToken);
}
