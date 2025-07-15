using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;

namespace Data.MongoDb.Repository;

public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(IMongoContext context)
        : base(context, "orderDetails")
    {
    }
}
