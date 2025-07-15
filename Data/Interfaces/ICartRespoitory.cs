using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface ICartRespoitory
{
    Task<IEnumerable<Cart>> GetCartAsync(CancellationToken cancellationToken);

    Task AddAsync(Cart cart, CancellationToken cancellationToken);

    Task<Cart> GetCartById(int id, CancellationToken cancellationToken);

    Task AddCartItemAsync(CartItem cartItem, CancellationToken cancellationToken);

    Task UpdateCartItemAsync(int cartItemId);

    void RemoveCartItem(CartItem cartItem);

    void RemoveCart(Cart cart);

    void UpdateAsync(CartItem cart);
}
