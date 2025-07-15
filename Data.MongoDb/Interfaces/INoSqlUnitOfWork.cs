namespace Data.MongoDb.Interfaces;

public interface INoSqlUnitOfWork
{
    IProductRepository ProductRepository { get; }

    IOrderRepository OrderRepository { get; }

    ISupplierRepository SupplierRepository { get; }

    ICategoryRepository CategoryRepository { get; }

    IShipperRepository ShipperRepository { get; }

    IOrderDetailRepository OrderDetailRepository { get; }

    ICustomerRepository CustomerRepository { get; }

    Task Save();
}
