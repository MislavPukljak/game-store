using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;

namespace Data.SQL.Repository;

public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(GameStoreContext context)
        : base(context)
    {
    }
}