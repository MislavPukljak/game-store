using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;

namespace Data.MongoDb.Repository;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(IMongoContext context)
        : base(context, "customers")
    {
    }
}
