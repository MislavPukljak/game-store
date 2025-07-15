using Business.DTO;

namespace Business.Interfaces;

public interface ICartService
{
    Task<IEnumerable<CartDto>> GetCartAsync(CancellationToken cancellationToken);

    Task<CartDto> GetCartById(int id, CancellationToken cancellationToken);

    Task AddCartItemAsync(int cartId, string key, CancellationToken cancellationToken);

    Task UpdateCartItemAsync(int cartItemId);

    Task RemoveCartItemAsync(int cartId, string key, CancellationToken cancellationToken);
}
