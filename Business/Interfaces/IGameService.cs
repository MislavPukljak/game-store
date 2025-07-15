using Business.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Business.Interfaces;

public interface IGameService
{
    Task<GameDto> AddAsync(CreateGameDto model, CancellationToken cancellationToken);

    Task<GameDto> GetByKeyAsync(string key, CancellationToken cancellationToken);

    Task<GameWithCountDto> GetAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<GameDto>> GetAllDeletedAsync(CancellationToken cancellationToken);

    Task DeleteAsync(string key, CancellationToken cancellationToken);

    Task<GameDto> UpdateAsync(GameDto game, CancellationToken cancellationToken);

    Task<FileStreamResult> DownloadGameAsync(string alias, CancellationToken cancellationToken);

    Task<IEnumerable<GameDto>> GetGamesByPlatformId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<GameDto>> GetGamesByGenreId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<GameDto>> GetGamesByPublisherId(int id, CancellationToken cancellationToken);

    Task<IEnumerable<GameDto>> GetGamesByPublisherName(string companyName, CancellationToken cancellationToken);

    Task<GameDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task AddCommentToGameAsync(string key, CommentDto comment, string userName, CancellationToken cancellationToken);

    Task ReplayToGameCommentAsync(int id, CommentDto comment, CancellationToken cancellationToken);

    Task QuoteToGameCommentAsync(int id, CommentDto comment, CancellationToken cancellationToken);

    Task<IEnumerable<CommentDto>> GetCommentsByGameAlias(string key, CancellationToken cancellationToken);

    Task<CommentDto> RemoveComment(int id, CancellationToken cancellationToken);

    Task<GameProductPageInfo> GetGamesAsync(GameFilterDto filter, CancellationToken cancellationToken);

    Task<GameDto> UpdateDeletedGamesAsync(GameDto game, CancellationToken cancellationToken);

    Task AddCommentToDeletedGameAsync(string key, CommentDto comment, CancellationToken cancellationToken);

    Task AddImageToGameAsync(string key, string fileName, CancellationToken cancellationToken);
}
