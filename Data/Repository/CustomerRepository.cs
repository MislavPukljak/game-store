using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;

namespace Data.SQL.Repository;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(GameStoreContext context)
        : base(context)
    {
    }
}
