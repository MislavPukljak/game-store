using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Data.SQL.Entities.Customer;

namespace WebAPI.Controllers;

[Route("api/customers")]
[ApiController]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("new")]
    public async Task<CustomerDto> AddNewCustomerAsync([FromBody] CustomerDto customerDto, CancellationToken cancellationToken = default)
    {
        return await _customerService.CreateCustomerAsync(customerDto, cancellationToken);
    }

    [HttpGet]
    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _customerService.GetAllCustomersAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CustomerDto> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _customerService.GetCustomerByIdAsync(id, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<CustomerDto> UpdateCustomerAsync([FromBody] CustomerDto customerDto, CancellationToken cancellationToken = default)
    {
        return await _customerService.UpdateCustomerAsync(customerDto, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task<CustomerDto> DeleteCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _customerService.DeleteCustomerAsync(id, cancellationToken);
    }

    [HttpPost("{id}/baned")]
    public async Task<CustomerDto> BanCustomerAsync(int id, BanDuration duration, CancellationToken cancellationToken = default)
    {
        return await _customerService.BanCustomer(id, duration, cancellationToken);
    }
}
