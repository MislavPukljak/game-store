using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface IPaymentOptionRepository
{
    Task<List<PaymentOption>> GetPaymentOptionAsync(CancellationToken cancellationToken);
}
