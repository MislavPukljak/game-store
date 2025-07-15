using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Data.MongoDb.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Logging;
using static Data.SQL.Entities.Customer;

namespace Business.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, INoSqlUnitOfWork sqlUnitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _noSqlUnitOfWork = sqlUnitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken)
    {
        var customers = await _unitOfWork.CustomerRepository.GetAllAsync(cancellationToken);
        var customersMongo = await _noSqlUnitOfWork.CustomerRepository.GetAllAsync(cancellationToken);

        var customerEntity = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        var customerMongo = _mapper.Map<IEnumerable<CustomerDto>>(customersMongo);

        return customerEntity.Concat(customerMongo);
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer by id: {id}", id);

        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning("Customer with id: {id} was not found", id);

            throw new CustomerException("Customer not found");
        }

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken cancellationToken)
    {
        var customer = _mapper.Map<Customer>(customerDto);
        await _unitOfWork.CustomerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Customer with id: {id} was created", customer.Id);

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating customer with id: {id}", customer.Id);

        var parseId = int.Parse(customer.Id.ToString());

        var customerExists = await _unitOfWork.CustomerRepository.GetByIdAsync(parseId, cancellationToken);

        if (customerExists is not null)
        {
            var customerEntity = _mapper.Map<Customer>(customer);

            _unitOfWork.CustomerRepository.Update(customerEntity);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Customer with id: {id} was updated", customer.Id);

            return _mapper.Map<CustomerDto>(customer);
        }

        _logger.LogError("Customer with id: {id} was not found", customer.Id);

        throw new CustomerException("Customer does not exist");
    }

    public async Task<CustomerDto> DeleteCustomerAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer with id: {id}", id);

        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            _logger.LogError("Customer with id: {id} was not found", id);

            throw new CustomerException("Customer does not exist");
        }

        _unitOfWork.CustomerRepository.DeleteAsync(customer, cancellationToken);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> BanCustomer(int customerId, BanDuration duration, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken) ?? throw new ArgumentException("Customer does not exist");
        switch (duration)
        {
            case BanDuration.OneHour:
                customer.BanedUntil = DateTime.UtcNow.AddHours(1);
                break;
            case BanDuration.OneDay:
                customer.BanedUntil = DateTime.UtcNow.AddDays(1);
                break;
            case BanDuration.OneWeek:
                customer.BanedUntil = DateTime.UtcNow.AddDays(7);
                break;
            case BanDuration.OneMonth:
                customer.BanedUntil = DateTime.UtcNow.AddMonths(1);
                break;
            case BanDuration.Forever:
                customer.IsPermanentlyBaned = true;
                customer.BanedUntil = null;
                break;
            default:
                customer.IsPermanentlyBaned = false;
                break;
        }

        _unitOfWork.CustomerRepository.Update(customer);

        await _unitOfWork.SaveAsync();

        return _mapper.Map<CustomerDto>(customer);
    }
}
