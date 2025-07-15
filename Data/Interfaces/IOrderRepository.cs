using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order> CreateOrderAsync(bool pay, int customerId, CancellationToken cancellationToken);

    Task EmptyCartAsync(int cartId, CancellationToken cancellationToken);

    Task CreateOrderDetailsAsync(List<OrderDetail> orderDetail, CancellationToken cancellationToken);

    Task<IEnumerable<Order>> GeOrderWithDetailsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Order>> GetOrderHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task<Order> GetOrderByIdAsync(int id, CancellationToken cancellationToken);
}
