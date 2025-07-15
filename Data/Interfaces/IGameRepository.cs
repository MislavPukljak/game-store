using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface IGameRepository : IGenericRepository<Game>
{
    Task<Game> GetGameByAlias(string key, CancellationToken cancellationToken);

    Task<Game> GetDeletedGameByAlias(string key, CancellationToken cancellationToken);

    Task<IEnumerable<Game>> GetGamesByPlatformId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Game>> GetGamesByGenreId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Game>> GetGamesByPublisherId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Game>> GetGamesByPublisher(string companyName, CancellationToken cancellationToken);

    Task<Game> GetGameByIdAsync(int id, CancellationToken cancellationToken);

    Task<Game> UpdateGameViewCount(Game game, CancellationToken cancellationToken);

    Task<GamePageInfo> GetGamesAsync(GameFilter filter, CancellationToken cancellationToken);

    Task<Game> GetGameByKey(string key, CancellationToken cancellationToken);

    Task<IEnumerable<Game>> GetAllDeletedGamesAsync(CancellationToken cancellationToken);

    Task<Game> GetGameByImageName(string fileName, CancellationToken cancellationToken);
}
