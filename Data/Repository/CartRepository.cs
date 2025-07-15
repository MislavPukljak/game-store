using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class CartRepository : ICartRespoitory
{
    private readonly GameStoreContext _context;

    public CartRepository(GameStoreContext context)
    {
        _context = context;
    }

    public async Task AddCartItemAsync(CartItem cartItem, CancellationToken cancellationToken)
    {
        await _context.CartItems.AddAsync(cartItem, cancellationToken);
    }

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken)
    {
        await _context.Set<Cart>().AddAsync(cart, cancellationToken);
    }

    public async Task<IEnumerable<Cart>> GetCartAsync(CancellationToken cancellationToken)
    {
        return await _context.Carts
        .AsNoTracking()
        .Include(i => i.CartItems)
        .ThenInclude(i => i.Products)
        .Select(i => new Cart
        {
            Id = i.Id,
            CartItems = i.CartItems.Select(ci => new CartItem
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                Price = ci.Price,
                Quantity = ci.Quantity,
                Discount = ci.Discount,
                Products = _context.Games.Where(g => g.Id == ci.ProductId).ToList(),
            }).ToList(),
        })
        .ToListAsync(cancellationToken);
    }

    public async Task<Cart> GetCartById(int id, CancellationToken cancellationToken)
    {
        return await _context.Carts
           .Include(i => i.CartItems)
           .Where(x => x.Id == id)
           .FirstOrDefaultAsync(cancellationToken);
    }

    public void RemoveCartItem(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }

    public void RemoveCart(Cart cart)
    {
        _context.Carts.Remove(cart);
    }

    public async Task UpdateCartItemAsync(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId, CancellationToken.None);

        if (cartItem != null)
        {
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
                _context.CartItems.Update(cartItem);
            }
            else
            {
                _context.CartItems.Remove(cartItem);
            }
        }
        else
        {
            throw new ArgumentException("CartItem not found");
        }
    }

    public void UpdateAsync(CartItem cart)
    {
        _context.Set<CartItem>().Update(cart);
    }
}
