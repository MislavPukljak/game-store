using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;

namespace Data.SQL.Repository;

public class VisaRepository : GenericRepository<Visa>, IVisaRepository
{
    public VisaRepository(GameStoreContext context)
        : base(context)
    {
    }
}
