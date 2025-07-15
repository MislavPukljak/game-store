using Data.MongoDb.Interfaces;
using Data.MongoDb.Repository;

namespace Data.MongoDb.Data;

public class NoSqlUnitOfWork : INoSqlUnitOfWork
{
    private readonly IMongoContext _context;

    public NoSqlUnitOfWork(IMongoContext context)
    {
        _context = context;
    }

    public IProductRepository ProductRepository => new ProductRepository(_context);

    public IOrderRepository OrderRepository => new OrderRepository(_context);

    public ISupplierRepository SupplierRepository => new SupplierRepository(_context);

    public ICategoryRepository CategoryRepository => new CategoryRepository(_context);

    public IShipperRepository ShipperRepository => new ShipperRepository(_context);

    public IOrderDetailRepository OrderDetailRepository => new OrderDetailRepository(_context);

    public ICustomerRepository CustomerRepository => new CustomerRepository(_context);

    public Task Save()
    {
        return _context.SaveChanges();
    }
}
