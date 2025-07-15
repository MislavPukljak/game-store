using System.Text;
using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Business.Resources;
using Data.MongoDb.Entities;
using Data.MongoDb.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Game = Data.SQL.Entities.Game;

namespace Business.Services;

public class GameService : IGameService
{
    private const string AllGames = "allGames";

    private readonly IUnitOfWork _unitOfWork;
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<GameService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IStringLocalizer<Resource> _localizer;

    public GameService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache memoryCache,
        ILogger<GameService> logger,
        INoSqlUnitOfWork noSqlUnitOfWork,
        UserManager<User> userManager,
        IStringLocalizer<Resource> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _logger = logger;
        _noSqlUnitOfWork = noSqlUnitOfWork;
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<GameDto> AddAsync(CreateGameDto model, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding a new game to the database");

        if (string.IsNullOrEmpty(model.Game.Key))
        {
            model.Game.Key = model.Game.Name.Replace(" ", "-").ToLower(System.Globalization.CultureInfo.CurrentCulture);
        }

        await IsKeyExisting(model.Game.Key, cancellationToken);

        var game = _mapper.Map<Game>(model.Game);

        game.Id = model.Game.Id;

        game.Genres = await _unitOfWork.GenreRepository.GetByIds(model.Genres, cancellationToken);
        game.Platforms = await _unitOfWork.PlatformRepository.GetByIds(model.Platforms, cancellationToken);
        game.Publishers = await _unitOfWork.PublisherRepository.GetByIdAsync(model.Publishers, cancellationToken);
        game.PublishedDate = DateTime.UtcNow;

        await _unitOfWork.GameRepository.AddAsync(game, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Game added to the database");

        return _mapper.Map<GameDto>(game);
    }

    public async Task<GameProductPageInfo> GetGamesAsync(GameFilterDto filter, CancellationToken cancellationToken)
    {
        var gamesDto = _mapper.Map<GameFilter>(filter);
        var productsDto = _mapper.Map<ProductFilter>(filter);

        var games = await _unitOfWork.GameRepository.GetGamesAsync(gamesDto, cancellationToken);
        var products = await _noSqlUnitOfWork.ProductRepository.GetProductsAsync(productsDto, cancellationToken);

        var gameEntities = _mapper.Map<GamePageInfoDto>(games);
        var productEntities = _mapper.Map<GamePageInfoDto>(products);

        var gameProducts = new GameProductPageInfo
        {
            GamesPageInfo = gameEntities,
            ProductsPageInfo = productEntities,
        };

        return gameProducts;
    }

    public async Task<GameDto> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting game by alias {key} from the database", key);

        var game = await _unitOfWork.GameRepository.GetGameByAlias(key, cancellationToken);
        var product = await _noSqlUnitOfWork.ProductRepository.GetByAliasAsync(key, cancellationToken);

        if (game is not null)
        {
            await _unitOfWork.GameRepository.UpdateGameViewCount(game, cancellationToken);

            var gameKey = _mapper.Map<GameDto>(game);

            await LocalizeAsync(gameKey);

            return gameKey;
        }
        else if (product is not null)
        {
            var productKey = _mapper.Map<GameDto>(product);

            return productKey;
        }

        _logger.LogError("Game with alias: {key} does not exists.", key);

        throw new GameException("Game not found");
    }

    public async Task<GameDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting game by id {id} from the database", id);

        var game = await _unitOfWork.GameRepository.GetGameByIdAsync(id, cancellationToken);
        var product = await _noSqlUnitOfWork.ProductRepository.GetByProductId(id, cancellationToken);

        if (game is not null)
        {
            await _unitOfWork.GameRepository.UpdateGameViewCount(game, cancellationToken);

            var gameEntity = _mapper.Map<GameDto>(game);

            return gameEntity;
        }
        else if (product is not null)
        {
            var productEntity = _mapper.Map<GameDto>(product);

            return productEntity;
        }

        _logger.LogError("Game with alias: {id} does not exists.", id);

        throw new GameException("Game not found");
    }

    public async Task<GameWithCountDto> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all games from the database");

        var games = await _unitOfWork.GameRepository.GetAllAsync(cancellationToken);
        var products = await _noSqlUnitOfWork.ProductRepository.GetAllAsync(cancellationToken);

        var gameModels = _mapper.Map<IEnumerable<GameDto>>(games);
        var productModels = _mapper.Map<IEnumerable<GameDto>>(products);

        var gameProducts = gameModels.Concat(productModels);

        var gamesCountSql = new GameWithCountDto
        {
            TotalCount = gameProducts.Count(),
            Games = gameProducts,
        };

        return gamesCountSql;
    }

    public async Task<IEnumerable<GameDto>> GetAllDeletedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all games from the database");

        var games = await _unitOfWork.GameRepository.GetAllDeletedGamesAsync(cancellationToken);

        var gameModels = _mapper.Map<IEnumerable<GameDto>>(games);

        return gameModels;
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting game with alias: {key}", key);

        var game = await _unitOfWork.GameRepository.GetGameByAlias(key, cancellationToken);
        if (game is not null)
        {
            await DeleteSqlAsync(key, cancellationToken);
            return;
        }

        var product = await _noSqlUnitOfWork.ProductRepository.GetByAliasAsync(key, cancellationToken);
        if (product is not null)
        {
            await DeleteMongoAsync(key, cancellationToken);
            return;
        }

        _logger.LogError("Game does not exists.");

        throw new GameException("Game not found");
    }

    public async Task<GameDto> UpdateAsync(GameDto game, CancellationToken cancellationToken)
    {
        var updateGame = await UpdateSqlAsync(game, cancellationToken);
        if (updateGame is not null)
        {
            return updateGame;
        }

        var updateProduct = await UpdateMongoAsync(game, cancellationToken);
        if (updateProduct is not null)
        {
            return updateProduct;
        }

        _logger.LogError("Game with alias: {key}, hasn't been found in db.", game.Key);

        throw new GameException("Game not found");
    }

    public async Task<GameDto> UpdateDeletedGamesAsync(GameDto game, CancellationToken cancellationToken)
    {
        var gameEntity = await _unitOfWork.GameRepository.GetDeletedGameByAlias(game.Key, cancellationToken);

        if (gameEntity is not null)
        {
            var gameModel = _mapper.Map<Game>(game);

            _unitOfWork.GameRepository.Update(gameModel);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Game: {game} updated in the SQL", game);

            return _mapper.Map<GameDto>(gameModel);
        }

        return null;
    }

    public async Task AddCommentToGameAsync(string key, CommentDto comment, string userName, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.GameRepository.GetGameByAlias(key, cancellationToken);

        var commentModel = _mapper.Map<Comment>(comment);

        commentModel.GameId = game.Id;

        var user = await UserNameExists(userName);

        comment.Name = user;

        await _unitOfWork.CommentRepository.AddAsync(commentModel, cancellationToken);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Comment added to the database");
    }

    public async Task AddCommentToDeletedGameAsync(string key, CommentDto comment, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.GameRepository.GetDeletedGameByAlias(key, cancellationToken);

        var commentModel = _mapper.Map<Comment>(comment);

        commentModel.GameId = game.Id;

        await _unitOfWork.CommentRepository.AddAsync(commentModel, cancellationToken);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Comment added to the database");
    }

    public async Task ReplayToGameCommentAsync(int id, CommentDto comment, CancellationToken cancellationToken)
    {
        var commentEntity = await _unitOfWork.CommentRepository.GetByIdAsync(id, cancellationToken);

        comment.ParentId = commentEntity.Id;
        comment.Body = $"[{commentEntity.Name}], {commentEntity.Body}\n{comment.Body}";

        var commentModel = _mapper.Map<Comment>(comment);

        commentModel.GameId = commentEntity.GameId;

        await _unitOfWork.CommentRepository.AddAsync(commentModel, cancellationToken);
        await _unitOfWork.SaveAsync();
    }

    public async Task QuoteToGameCommentAsync(int id, CommentDto comment, CancellationToken cancellationToken)
    {
        var commentEntity = await _unitOfWork.CommentRepository.GetByIdAsync(id, cancellationToken);

        comment.ParentId = commentEntity.Id;
        comment.Body = $"*{commentEntity.Body}*\n{comment.Body}";

        var commentModel = _mapper.Map<Comment>(comment);

        commentModel.GameId = commentEntity.GameId;

        await _unitOfWork.CommentRepository.AddAsync(commentModel, cancellationToken);
        await _unitOfWork.SaveAsync();
    }

    public async Task<FileStreamResult> DownloadGameAsync(string alias, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading game with alias: {alias}", alias);

        var gameByAlias = await _unitOfWork.GameRepository.GetGameByAlias(alias, cancellationToken);

        var mappedGame = _mapper.Map<GameDto>(gameByAlias);

        string? fileName;
        string? myString;

        if (gameByAlias is not null)
        {
            fileName = $"{mappedGame.Key}_{DateTime.UtcNow:yyyyMMddHHmm}.txt";

            myString = mappedGame.Description;
        }
        else
        {
            _logger.LogError("Game with alias: {alias}, hasn't been found in db.", alias);

            throw new GameException("Game not found");
        }

        var byteArray = Encoding.ASCII.GetBytes(myString);

        var stream = new MemoryStream(byteArray);

        return new FileStreamResult(stream, "text/plain")
        {
            FileDownloadName = fileName,
        };
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPlatformId(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all games from the database by platform id.");

        var cachedGames = _memoryCache.Get<IEnumerable<GameDto>>(AllGames);

        if (cachedGames is not null)
        {
            return cachedGames;
        }

        var games = await _unitOfWork.GameRepository.GetGamesByPlatformId(id, cancellationToken);

        if (games is not null)
        {
            var gameModels = _mapper.Map<IEnumerable<GameDto>>(games);

            return gameModels;
        }

        _logger.LogError("Game not found");

        throw new GameException("Games not found.");
    }

    public async Task<IEnumerable<GameDto>> GetGamesByGenreId(int id, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.GameRepository.GetGamesByGenreId(id, cancellationToken);
        var products = await _noSqlUnitOfWork.ProductRepository.GetProductByCategoryId(id, cancellationToken);

        if (games.Any())
        {
            var gameModels = _mapper.Map<IEnumerable<GameDto>>(games);

            return gameModels;
        }
        else if (products.Any())
        {
            var productModels = _mapper.Map<IEnumerable<GameDto>>(products);

            return productModels;
        }

        _logger.LogError("Game not found");

        throw new GameException("Games not found.");
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisherId(int id, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.GameRepository.GetGamesByPublisherId(id, cancellationToken);
        var products = await _noSqlUnitOfWork.ProductRepository.GetProductBySuplierId(id, cancellationToken);

        if (games.Any())
        {
            var gameModels = _mapper.Map<IEnumerable<GameDto>>(games);

            return gameModels;
        }
        else if (products.Any())
        {
            var productModels = _mapper.Map<IEnumerable<GameDto>>(products);

            return productModels;
        }

        _logger.LogError("Game not found");

        throw new GameException("Games not found.");
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisherName(string companyName, CancellationToken cancellationToken)
    {
        var games = await _unitOfWork.GameRepository.GetGamesByPublisher(companyName, cancellationToken);
        var products = await _noSqlUnitOfWork.ProductRepository.GetProductBySupplier(companyName, cancellationToken);

        if (games.Any())
        {
            var gameModels = _mapper.Map<IEnumerable<GameDto>>(games);

            return gameModels;
        }
        else if (products.Any())
        {
            var productModels = _mapper.Map<IEnumerable<GameDto>>(products);

            return productModels;
        }

        _logger.LogError("Game not found");

        throw new GameException("Games not found.");
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByGameAlias(string key, CancellationToken cancellationToken)
    {
        var comments = await _unitOfWork.CommentRepository.GetCommentsByGameAlias(key, cancellationToken);

        var commentsDto = _mapper.Map<IEnumerable<CommentDto>>(comments);

        var commentLookup = commentsDto.ToDictionary(c => c.Id, c => c);

        var topLevelComments = new List<CommentDto>();

        foreach (var commentDto in commentsDto)
        {
            if (commentDto.ParentId.HasValue)
            {
                if (commentLookup.TryGetValue(commentDto.ParentId.Value, out var parent))
                {
                    parent.ChildComments.Add(commentDto);
                }
            }
            else
            {
                topLevelComments.Add(commentDto);
            }
        }

        return topLevelComments;
    }

    public async Task<CommentDto> RemoveComment(int id, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.CommentRepository.RemoveComment(id, cancellationToken);

        if (comment is not null)
        {
            var commentModel = _mapper.Map<CommentDto>(comment);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Comment removed from the database");

            return commentModel;
        }

        _logger.LogError("Comment not found");

        throw new GameException("Comment not found.");
    }

    public async Task AddImageToGameAsync(string key, string fileName, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.GameRepository.GetGameByKey(key, cancellationToken);

        if (game is not null)
        {
            var image = await _unitOfWork.ImagesDataRepository.GetByNameAsync(fileName);

            if (image is null)
            {
                _logger.LogError("Image not found");

                throw new GameException("Image not found.");
            }

            game.Images ??= new List<ImagesData>();

            game.Images.Add(image);

            var gameEntity = _mapper.Map<Game>(game);

            _unitOfWork.GameRepository.Update(gameEntity);

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Image added to the database");
        }
        else
        {
            _logger.LogError("Game not found");

            throw new GameException("Game not found.");
        }
    }

    private async Task IsKeyExisting(string key, CancellationToken cancellationToken)
    {
        var game = await _unitOfWork.GameRepository.GetGameByAlias(key, cancellationToken);

        if (game is not null)
        {
            _logger.LogError("Game with alias: {key} already exists.", key);

            throw new GameException("Game alias already exists");
        }
    }

    private async Task DeleteSqlAsync(string key, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.GameRepository.GetGameByAlias(key, cancellationToken);

        entity.IsDeleted = true;

        _unitOfWork.GameRepository.Update(entity);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Game removed from the database");
    }

    private async Task DeleteMongoAsync(string alias, CancellationToken cancellationToken)
    {
        var product = await _noSqlUnitOfWork.ProductRepository.GetByAliasAsync(alias, cancellationToken);

        await _noSqlUnitOfWork.ProductRepository.DeleteProductAsync(alias, cancellationToken);

        _logger.LogInformation("Product: {product} removed from the database", product);
    }

    private async Task<GameDto> UpdateSqlAsync(GameDto game, CancellationToken cancellationToken)
    {
        var gameEntity = await _unitOfWork.GameRepository.GetGameByAlias(game.Key, cancellationToken);

        if (gameEntity is not null)
        {
            var gameModel = _mapper.Map<Game>(game);

            _unitOfWork.GameRepository.Update(gameModel);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Game: {game} updated in the SQL", game);

            return _mapper.Map<GameDto>(gameModel);
        }

        return null;
    }

    private async Task<GameDto> UpdateMongoAsync(GameDto game, CancellationToken cancellationToken)
    {
        var product = await _noSqlUnitOfWork.ProductRepository.GetByAliasAsync(game.Key, cancellationToken);

        if (product is not null)
        {
            var productModel = _mapper.Map<Game>(game);

            await _unitOfWork.GameRepository.AddAsync(productModel, cancellationToken);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Product: {product} updated in the MongoDb", product);

            var productDto = _mapper.Map<GameDto>(product);
            productDto.PublishedDate = DateTime.UtcNow;

            return productDto;
        }

        return null;
    }

    private async Task<string> UserNameExists(string userName)
    {
        if (userName is not null)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user is not null ? user.UserName : userName;
        }

        _logger.LogError("User not found");

        throw new GameException("User not found.");
    }

    private async Task<GameDto> LocalizeAsync(GameDto game)
    {
        foreach (var platform in game.Platforms)
        {
            platform.Type = _localizer[platform.Type].Value;
        }

        foreach (var genre in game.Genres)
        {
            genre.Name = _localizer[genre.Name].Value;
        }

        return await Task.FromResult(game);
    }
}