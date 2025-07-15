using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("new")]
    public async Task<ActionResult<CartDto>> AddAsync(int cartId, string key, CancellationToken cancellationToken = default)
    {
        await _cartService.AddCartItemAsync(cartId, key, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = cartId }, cartId);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cart = await _cartService.GetCartById(id, cancellationToken);

        return Ok(cart);
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCartAsync(CancellationToken cancellationToken = default)
    {
        var cart = await _cartService.GetCartAsync(cancellationToken);

        return Ok(cart);
    }

    [HttpDelete("remove")]
    public async Task<ActionResult<CartDto>> RemoveCartItem(int cartId, string key, CancellationToken cancellationToken)
    {
        await _cartService.RemoveCartItemAsync(cartId, key, cancellationToken);

        return Ok();
    }

    [HttpPut("update")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(int cartItemId)
    {
        await _cartService.UpdateCartItemAsync(cartItemId);

        return Ok();
    }
}
