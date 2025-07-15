using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Data.MongoDb.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;
    private readonly INotificationService _notificationService;

    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OrderService> logger,
        INoSqlUnitOfWork noSqlUnitOfWork,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _noSqlUnitOfWork = noSqlUnitOfWork;
        _notificationService = notificationService;
    }

    public async Task CreateOrderAsync(bool pay, int cartId, int customerId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Creating order for customer with id: {customerId}");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var order = await _unitOfWork.OrderRepository.CreateOrderAsync(pay, customerId, cancellationToken);

            await _unitOfWork.SaveAsync();

            var cart = await _unitOfWork.CartRepository.GetCartById(cartId, cancellationToken);

            var orderDetails = cart.CartItems.Select(x => new OrderDetail
            {
                OrderId = order.Id,
                ProductId = x.ProductId,
                Price = x.Price,
                Quantity = x.Quantity,
            }).ToList();

            order.Sum = cart.CartItems.Sum(od => od.Price * od.Quantity);

            order.Status = OrderStatus.Open;

            await _unitOfWork.OrderRepository.CreateOrderDetailsAsync(orderDetails, cancellationToken);

            await _unitOfWork.OrderRepository.EmptyCartAsync(cartId, cancellationToken);

            await _unitOfWork.SaveAsync();

            await SendNotification(order.Id, cancellationToken);

            transaction.Commit();

            _logger.LogInformation($"Order with id: {order.Id} created");
        }
        catch (Exception)
        {
            transaction.Rollback();

            _logger.LogError($"Order for customer with id: {customerId} failed");

            throw;
        }
    }

    public async Task DeleteOrderAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deleting order with id: {id}");

        var orderEntity = await _unitOfWork.OrderRepository.GetByIdAsync(id, cancellationToken);

        if (orderEntity is null)
        {
            _logger.LogError($"Order with id: {id} not found");

            throw new OrderException($"Order with id: {id} not found");
        }

        _unitOfWork.OrderRepository.DeleteAsync(orderEntity, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Order with id: {id} deleted");
    }

    public async Task<IEnumerable<OrderDto>> GetOrderWithDetailsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all orders");

        var orders = await _unitOfWork.OrderRepository.GeOrderWithDetailsAsync(cancellationToken);

        if (orders is not null)
        {
            var order = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return order;
        }

        _logger.LogError("No orders found");

        throw new OrderException("No orders found");
    }

    public async Task<IEnumerable<OrderHistoryDto>> GetOrderHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting order history from {startDate} to {endDate}");

        if (endDate == default)
        {
            endDate = DateTime.UtcNow.AddDays(-30);
        }

        var orders = await _unitOfWork.OrderRepository.GetOrderHistoryAsync(startDate.ToUniversalTime(), endDate.ToUniversalTime(), cancellationToken);
        var orderMongo = await _noSqlUnitOfWork.OrderRepository.GetOrdersHistoryAsync(startDate, endDate, cancellationToken);

        var order = _mapper.Map<IEnumerable<OrderHistoryDto>>(orders);
        var orderHistory = _mapper.Map<IEnumerable<OrderHistoryDto>>(orderMongo);

        return order.Concat(orderHistory);
    }

    public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting order with id: {id}");

        var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(id, cancellationToken);

        if (order is not null)
        {
            var orderDto = _mapper.Map<OrderDto>(order);

            return orderDto;
        }

        _logger.LogError($"No order found with id: {id}");

        throw new OrderException($"No order found with id: {id}");
    }

    public async Task UpdateOrderStatusAsync(int id, OrderStatus status, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating order status.");

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var orderEntity = await _unitOfWork.OrderRepository.GetByIdAsync(id, cancellationToken);

            if (orderEntity is null)
            {
                _logger.LogError($"Order with id: {id} not found");

                throw new OrderException($"Order with id: {id} not found");
            }

            orderEntity.Status = status;

            _unitOfWork.OrderRepository.Update(orderEntity);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation($"Order with id: {id} updated");
        }
        catch (Exception)
        {
            transaction.Rollback();

            _logger.LogError($"Order with id: {id} failed");

            throw;
        }
    }

    private async Task SendNotification(int id, CancellationToken cancellationToken)
    {
        var order = await GetOrderByIdAsync(id, cancellationToken);

        await _notificationService.SendOrderEmailAsync(order, cancellationToken);
        await _notificationService.SendOrderSmsAsync(order, cancellationToken);
        await _notificationService.SendOrderNotificationAsync(order, cancellationToken);
    }
}
