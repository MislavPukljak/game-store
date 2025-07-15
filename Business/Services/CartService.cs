using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CartService> _logger;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper, INoSqlUnitOfWork noSqlUnitOfWork, ILogger<CartService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _noSqlUnitOfWork = noSqlUnitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<CartDto>> GetCartAsync(CancellationToken cancellationToken)
    {
        var cartEntity = await _unitOfWork.CartRepository.GetCartAsync(cancellationToken);

        return _mapper.Map<IEnumerable<CartDto>>(cartEntity);
    }

    public async Task<CartDto> GetCartById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting cart with id {id}");

        var cart = await _unitOfWork.CartRepository.GetCartById(id, cancellationToken);

        if (cart is not null)
        {
            var cartDto = _mapper.Map<CartDto>(cart);

            return cartDto;
        }

        _logger.LogError($"Cart with id {id} not found");

        throw new CartException($"Cart with id {id} not found");
    }

    public async Task RemoveCartItemAsync(int cartId, string key, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.CartRepository.GetCartById(cartId, cancellationToken);

        var game = await _unitOfWork.GameRepository.GetGameByKey(key, cancellationToken);
        var product = await _noSqlUnitOfWork.ProductRepository.GetByAliasAsync(key, cancellationToken);

        RemoveCartItem(cart, game.Id);

        await IncreaseProductStock(game, product, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Cart item with id {game.Id} removed");
    }

    public async Task UpdateCartItemAsync(int cartItemId)
    {
        _logger.LogInformation($"Updating cart item with id {cartItemId}");

        await _unitOfWork.CartRepository.UpdateCartItemAsync(cartItemId);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Cart item with id {cartItemId} updated");
    }

    public async Task AddCartItemAsync(int cartId, string key, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding game with alias {key} to cart with id {cartId}");

        var cart = await GetOrCreateCart(cartId, cancellationToken);

        var game = await _unitOfWork.GameRepository.GetGameByKey(key, cancellationToken);
        var product = await _noSqlUnitOfWork.ProductRepository.GetByAliasAsync(key, cancellationToken);

        var existingCartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == game.Id);

        if (existingCartItem is not null)
        {
            UpdateCartItem(existingCartItem);
            await ReduceProductStock(game, product, cancellationToken);
        }
        else
        {
            if (game is not null)
            {
                await AddNewGameCartItem(cart, game, cancellationToken);
                await ReduceProductStock(game, product, cancellationToken);
            }
            else if (product is not null)
            {
                await AddNewProductCartItem(cart, product, cancellationToken);
            }
        }

        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Game with alias {key} added to cart with id {cartId}");
    }

    private async Task<Cart> GetOrCreateCart(int cartId, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.CartRepository.GetCartById(cartId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                Id = cartId,
                CartItems = new List<CartItem>(),
            };
            await _unitOfWork.CartRepository.AddAsync(cart, cancellationToken);
        }

        return cart;
    }

    private void UpdateCartItem(CartItem existingCartItem)
    {
        existingCartItem.Quantity++;
        _unitOfWork.CartRepository.UpdateAsync(existingCartItem);
    }

    private async Task ReduceProductStock(Game game, Product product, CancellationToken cancellationToken)
    {
        if (game is not null && product is not null)
        {
            game.UnitInStock--;
            product.UnitsInStock--;

            await _noSqlUnitOfWork.ProductRepository.UpdateUnitInStock(product.Alias, product, cancellationToken);
        }
        else if (game is not null)
        {
            game.UnitInStock--;
        }

        var gameEntity = _mapper.Map<Game>(game);
        _unitOfWork.GameRepository.Update(gameEntity);
    }

    private async Task IncreaseProductStock(Game game, Product product, CancellationToken cancellationToken)
    {
        if (game is not null && product is not null)
        {
            game.UnitInStock++;
            product.UnitsInStock++;

            await _noSqlUnitOfWork.ProductRepository.UpdateUnitInStock(product.Alias, product, cancellationToken);
        }
        else if (game is not null)
        {
            game.UnitInStock++;
        }

        var gameEntity = _mapper.Map<Game>(game);
        _unitOfWork.GameRepository.Update(gameEntity);
    }

    private async Task AddNewGameCartItem(Cart cart, Game game, CancellationToken cancellationToken)
    {
        var cartItem = new CartItemsDto
        {
            ProductId = game.Id,
            Price = game.Price,
            Quantity = 1,
        };
        var cartItemEntity = _mapper.Map<CartItem>(cartItem);

        cart.CartItems.Add(cartItemEntity);
        await _unitOfWork.CartRepository.AddCartItemAsync(cartItemEntity, cancellationToken);
    }

    private async Task AddNewProductCartItem(Cart cart, Product product, CancellationToken cancellationToken)
    {
        var cartItem = new CartItemsDto
        {
            ProductId = product.ProductID,
            Price = product.UnitPrice,
            Quantity = 1,
        };
        var cartItemEntity = _mapper.Map<CartItem>(cartItem);

        cart.CartItems.Add(cartItemEntity);
        await _unitOfWork.CartRepository.AddCartItemAsync(cartItemEntity, cancellationToken);

        product.UnitsInStock--;
        await _noSqlUnitOfWork.ProductRepository.UpdateUnitInStock(product.Alias, product, cancellationToken);

        var productDto = _mapper.Map<GameDto>(product);
        productDto.Id = 0;
        productDto.PublishedDate = DateTime.UtcNow;
        var productEntity = _mapper.Map<Game>(productDto);

        await _unitOfWork.GameRepository.AddAsync(productEntity, cancellationToken);
    }

    private void RemoveCartItem(Cart cart, int id)
    {
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == id);

        if (cartItem is null)
        {
            _logger.LogError($"Cart item with id {id} not found");

            throw new CartException($"Cart item with id {id} not found");
        }

        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;

            _unitOfWork.CartRepository.UpdateAsync(cartItem);

            return;
        }

        _unitOfWork.CartRepository.RemoveCartItem(cartItem);
        _unitOfWork.CartRepository.RemoveCart(cart);
    }
}
