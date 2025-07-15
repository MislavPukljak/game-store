using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Enums;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class GameRepository : GenericRepository<Game>, IGameRepository
{
    private readonly DbSet<Game> _dbSet;
    private readonly GameStoreContext _context;

    public GameRepository(GameStoreContext context)
        : base(context)
    {
        _dbSet = context.Set<Game>();
        _context = context;
    }

    public override async Task AddAsync(Game entity, CancellationToken cancellationToken)
    {
        await _context.Games.AddAsync(entity, cancellationToken);
    }

    public override async Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Games
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetAllDeletedGamesAsync(CancellationToken cancellationToken)
    {
        return await _context.Games
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Game> GetGameByIdAsync(int id, CancellationToken cancellationToken)
    {
        var games = await _dbSet
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        return games;
    }

    public async Task<GamePageInfo> GetGamesAsync(GameFilter filter, CancellationToken cancellationToken)
    {
        var games = _dbSet
            .Include(g => g.Publishers)
            .Include(g => g.Platforms)
            .Include(g => g.Genres)
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        games = FilterByGenres(games, filter.Genres);
        games = FilterByPlatforms(games, filter.Platforms);
        games = FilterByPublishers(games, filter.Publishers);
        games = FilterByPrice(games, filter.PriceFrom, filter.PriceTo);
        games = FilterByPublishedDate(games, filter.PublishedDate);
        games = FilterByNameStart(games, filter.NameStart);
        games = SortGames(games, filter.Sort);

        var selectedGames = await games
            .Skip((filter.CurrentPage - 1) * (int)filter.ItemsPerPage)
            .Take((int)filter.ItemsPerPage)
            .ToListAsync(cancellationToken);

        var response = new GamePageInfo
        {
            Games = selectedGames,
            CurrentPage = filter.CurrentPage,
            TotalPages = (int)Math.Ceiling((decimal)games.Count() / (int)filter.ItemsPerPage),
            TotalItems = games.Count(),
            PageSize = (int)filter.ItemsPerPage,
        };

        return response;
    }

    public async Task<Game> GetGameByAlias(string key, CancellationToken cancellationToken)
       => await _dbSet
        .AsNoTracking()
        .Include(g => g.Publishers)
        .Include(g => g.Platforms)
        .Include(g => g.Genres)
        .Include(g => g.Comments)
            .ThenInclude(c => c.ChildComments)
        .Include(g => g.Images)
        .Where(x => x.Alias == key)
        .Where(x => !x.IsDeleted)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Game> GetDeletedGameByAlias(string key, CancellationToken cancellationToken)
       => await _dbSet
        .AsNoTracking()
        .Include(g => g.Publishers)
        .Include(g => g.Platforms)
        .Include(g => g.Genres)
        .Include(g => g.Comments)
            .ThenInclude(c => c.ChildComments)
        .Where(x => x.Alias == key)
        .Where(x => x.IsDeleted)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Game> GetGameByKey(string key, CancellationToken cancellationToken)
       => await _dbSet
        .AsNoTracking()
        .Where(x => x.Alias == key)
        .Where(x => !x.IsDeleted)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Game>> GetGamesByPlatformId(int id, CancellationToken cancellationToken)
        => await _dbSet
        .AsNoTracking()
        .Where(x => x.Platforms.Any(p => p.Id == id))
        .Where(x => !x.IsDeleted)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Game>> GetGamesByGenreId(int id, CancellationToken cancellationToken)
        => await _dbSet
        .AsNoTracking()
        .Where(x => x.Genres.Any(p => p.Id == id))
        .Where(x => !x.IsDeleted)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Game>> GetGamesByPublisherId(int id, CancellationToken cancellationToken)
        => await _dbSet
        .AsNoTracking()
        .Where(x => x.Publishers.Id == id)
        .Where(x => !x.IsDeleted)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Game>> GetGamesByPublisher(string companyName, CancellationToken cancellationToken)
        => await _dbSet
        .AsNoTracking()
        .Include(g => g.Publishers)
        .Where(x => x.Publishers.CompanyName == companyName)
        .Where(x => !x.IsDeleted)
        .ToListAsync(cancellationToken);

    public async Task<Game> UpdateGameViewCount(Game game, CancellationToken cancellationToken)
    {
        var gameGenre = game.Genres;
        var gamePlatform = game.Platforms;
        var gamePublisher = game.Publishers;
        var gameImages = game.Images;

        game.Platforms = null;
        game.Genres = null;
        game.Publishers = null;
        game.Images = null;
        game.Views++;

        Update(game);

        await _context.SaveChangesAsync(cancellationToken);

        game.Genres = gameGenre;
        game.Platforms = gamePlatform;
        game.Publishers = gamePublisher;
        game.Images = gameImages;

        return game;
    }

    public async Task<Game> GetGameByImageName(string fileName, CancellationToken cancellationToken)
    {
        var games = await _dbSet
        .AsNoTracking()
        .Where(x => !x.IsDeleted && x.Images != null)
        .FirstOrDefaultAsync(cancellationToken);

        return games;
    }

    private static IQueryable<Game> FilterByGenres(IQueryable<Game> games, List<string>? genres)
    {
        return genres == null || !genres.Any() ? games : games.Where(game => game.Genres.Any(genre => genres.Contains(genre.Name)));
    }

    private static IQueryable<Game> FilterByPlatforms(IQueryable<Game> games, List<string>? platforms)
    {
        return platforms?.Any() == true ? games.Where(game => game.Platforms.Any(platform => platforms.Contains(platform.Type))) : games;
    }

    private static IQueryable<Game> FilterByPublishers(IQueryable<Game> games, string? publisher)
    {
        return !string.IsNullOrEmpty(publisher) ? games.Where(game => game.Publishers.CompanyName == publisher) : games;
    }

    private static IQueryable<Game> FilterByPrice(IQueryable<Game> games, decimal? priceFrom, decimal? priceTo)
    {
        if (priceFrom.HasValue)
        {
            return games.Where(game => game.Price >= priceFrom);
        }
        else if (priceTo.HasValue)
        {
            return games.Where(game => game.Price <= priceTo);
        }

        return games;
    }

    private static IQueryable<Game> FilterByPublishedDate(IQueryable<Game> games, Time.TimeRange? publishedDate)
    {
        if (publishedDate.HasValue)
        {
            var currentDate = DateTime.UtcNow;

            return publishedDate switch
            {
                Time.TimeRange.LastWeek => games.Where(game => game.PublishedDate >= currentDate.AddDays(-7)),
                Time.TimeRange.LastMonth => games.Where(game => game.PublishedDate >= currentDate.AddMonths(-1)),
                Time.TimeRange.LastYear => games.Where(game => game.PublishedDate >= currentDate.AddYears(-1)),
                Time.TimeRange.TwoYearsAgo => games.Where(game => game.PublishedDate >= currentDate.AddYears(-2)),
                Time.TimeRange.ThreeYearsAgo => games.Where(game => game.PublishedDate >= currentDate.AddYears(-3)),
                _ => games,
            };
        }

        return games;
    }

    private static IQueryable<Game> FilterByNameStart(IQueryable<Game> games, string? nameStart)
    {
        return !string.IsNullOrEmpty(nameStart) ? games.Where(game => game.Name.Contains(nameStart)) : games;
    }

    private static IQueryable<Game> SortGames(IQueryable<Game> games, OrderFilter.OrderBy? sort)
    {
        return sort switch
        {
            OrderFilter.OrderBy.MostPopular => games.OrderByDescending(game => game.Views),
            OrderFilter.OrderBy.MostCommented => games.OrderByDescending(game => game.Comments.Count),
            OrderFilter.OrderBy.PriceAsc => games.OrderBy(game => game.Price),
            OrderFilter.OrderBy.PriceDesc => games.OrderByDescending(game => game.Price),
            OrderFilter.OrderBy.ByDate => games.OrderByDescending(game => game.PublishedDate),
            _ => games.OrderByDescending(game => game.PublishedDate),
        };
    }
}
