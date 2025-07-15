using Data.MongoDb.Entities;

namespace Data.MongoDb.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> GetOrdersHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
