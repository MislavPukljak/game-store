using Business.DTO;
using static Data.SQL.Entities.Customer;

namespace Business.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken);

    Task<CustomerDto> GetCustomerByIdAsync(int id, CancellationToken cancellationToken);

    Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken cancellationToken);

    Task<CustomerDto> UpdateCustomerAsync(CustomerDto customer, CancellationToken cancellationToken);

    Task<CustomerDto> DeleteCustomerAsync(int id, CancellationToken cancellationToken);

    Task<CustomerDto> BanCustomer(int customerId, BanDuration duration, CancellationToken cancellationToken);
}
