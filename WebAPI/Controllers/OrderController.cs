using Business.DTO;
using Business.Interfaces;
using Data.SQL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize(Roles = "Administrator, Manager")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderServices;
    private readonly IPaymentOptionService _paymentOptionService;

    public OrderController(
        IOrderService orderServices,
        IPaymentOptionService paymentOptionService)
    {
        _orderServices = orderServices;
        _paymentOptionService = paymentOptionService;
    }

    // POST: api/orders/new
    [HttpPost("new")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> CreateOrder(bool pay, int cartId, int customerId, CancellationToken cancellationToken = default)
    {
        await _orderServices.CreateOrderAsync(pay, cartId, customerId, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = cartId }, cartId);
    }

    // GET: api/orders
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderServices.GetOrderWithDetailsAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderServices.GetOrderByIdAsync(id, cancellationToken);

        return Ok(order);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersHistoryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var orders = await _orderServices.GetOrderHistoryAsync(startDate, endDate, cancellationToken);

        return Ok(orders);
    }

    [HttpDelete("remove/{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _orderServices.DeleteOrderAsync(id, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<object>> MakePayment(int orderId, string paymentOptionTitle, VisaDto visa)
    {
        var invoice = await _paymentOptionService.HandlePaymentAsync(orderId, paymentOptionTitle, visa);

        return paymentOptionTitle == "Bank"
            ? (ActionResult<object>)File(
                    (byte[])invoice,
                    "application/octet-stream",
                    "Invoice-" + orderId + ".pdf")
            : (ActionResult<object>)Ok(invoice);
    }

    [HttpPut("status")]
    public async Task<ActionResult> UpdateOrderStatus(int orderId, OrderStatus status, CancellationToken cancellationToken)
    {
        await _orderServices.UpdateOrderStatusAsync(orderId, status, cancellationToken);

        return Ok();
    }
}
