using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;
using MongoDB.Driver;

namespace Data.MongoDb.Repository;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly IMongoCollection<Order> _dbSet;

    public OrderRepository(IMongoContext context)
        : base(context, "orders")
    {
        _dbSet = context.GetCollection<Order>("orders");
    }

    public Task<List<Order>> GetOrdersHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var filter = Builders<Order>.Filter.Gte("OrderDate", startDate) &
                     Builders<Order>.Filter.Lte("OrderDate", endDate);

        var orders = _dbSet
            .Find(filter)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
