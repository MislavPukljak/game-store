using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly GameStoreContext _context;

    public OrderRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(bool pay, int customerId, CancellationToken cancellationToken)
    {
        if (pay)
        {
            var newOrder = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                CreationDate = DateTime.UtcNow,
                PaidOrder = DateTime.UtcNow,
                OrderDetails = new List<OrderDetail>(),
            };

            await _context.Orders.AddAsync(newOrder, cancellationToken);

            return newOrder;
        }

        return null;
    }

    public async Task CreateOrderDetailsAsync(List<OrderDetail> orderDetail, CancellationToken cancellationToken)
    {
        await _context.OrderDetails.AddRangeAsync(orderDetail, cancellationToken);
    }

    public async Task EmptyCartAsync(int cartId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);

        if (cart is null)
        {
            return;
        }

        _context.Carts.Remove(cart);
    }

    public async Task<IEnumerable<Order>> GeOrderWithDetailsAsync(CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .Where(x => x.OrderDate >= DateTime.UtcNow.AddDays(-30))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrderHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}