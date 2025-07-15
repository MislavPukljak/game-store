using Business.DTO;
using Data.SQL.Entities;

namespace Business.Interfaces;

public interface IOrderService
{
    Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken);

    Task CreateOrderAsync(bool pay, int cartId, int customerId, CancellationToken cancellationToken);

    Task DeleteOrderAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<OrderDto>> GetOrderWithDetailsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<OrderHistoryDto>> GetOrderHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task UpdateOrderStatusAsync(int id, OrderStatus status, CancellationToken cancellationToken);
}
