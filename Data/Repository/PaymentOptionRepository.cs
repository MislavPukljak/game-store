using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class PaymentOptionRepository : IPaymentOptionRepository
{
    private readonly GameStoreContext _context;

    public PaymentOptionRepository(GameStoreContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentOption>> GetPaymentOptionAsync(CancellationToken cancellationToken)
    {
        return await _context.PaymentOptions.ToListAsync(cancellationToken);
    }
}
