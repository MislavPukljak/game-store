using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/games")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameServices;

    public GameController(IGameService gameServices)
    {
        _gameServices = gameServices;
    }

    // POST: api/games/new
    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameDto>> AddAsync([FromBody] CreateGameDto game, CancellationToken cancellationToken = default)
    {
        await _gameServices.AddAsync(game, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = game.Game.Id }, game);
    }

    // GET: api/games
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GameWithCountDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetAllAsync(cancellationToken);

        return Ok(games);
    }

    // GET: api/games/all
    [HttpGet("deletedGames")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAllDeletedGamesAsync(CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetAllDeletedAsync(cancellationToken);

        return Ok(games);
    }

    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GameDto>>> GetAllGamesAsync([FromQuery] GameFilterDto gameFilter, CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetGamesAsync(gameFilter, cancellationToken);

        return Ok(games);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<GameDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var game = await _gameServices.GetByIdAsync(id, cancellationToken);

        return Ok(game);
    }

    // GET: api/games/{key}
    [HttpGet("{key}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetByAliasAsync(string key, CancellationToken cancellationToken = default)
    {
        var game = await _gameServices.GetByKeyAsync(key, cancellationToken);

        return Ok(game);
    }

    [HttpGet("{key}/download")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> DownloadGameAsync(string key, CancellationToken cancellationToken = default)
    {
        var fileResult = await _gameServices.DownloadGameAsync(key, cancellationToken);

        return File(fileResult.FileStream, fileResult.ContentType, fileResult.FileDownloadName);
    }

    [HttpGet("genres/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByGenreId(int id, CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetGamesByGenreId(id, cancellationToken);

        return Ok(games);
    }

    [HttpGet("platforms/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetGamesByPlatformId(int id, CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetGamesByPlatformId(id, cancellationToken);

        return Ok(games);
    }

    [HttpGet("publishers/{companyName}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetGamesByPublisherName(string companyName, CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetGamesByPublisherName(companyName, cancellationToken);

        return Ok(games);
    }

    [HttpGet("publishers/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetGamesByPublisherId(int id, CancellationToken cancellationToken = default)
    {
        var games = await _gameServices.GetGamesByPublisherId(id, cancellationToken);

        return Ok(games);
    }

    [HttpGet("{key}/comments")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ObjectResult> GetCommentsByGameAliasAsync(string key, CancellationToken cancellationToken = default)
    {
        var comments = await _gameServices.GetCommentsByGameAlias(key, cancellationToken);

        return Ok(comments);
    }

    // DELETE: api/games/remove
    [HttpDelete("remove/{key}")]
    [Authorize("Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        await _gameServices.DeleteAsync(key, cancellationToken);

        return Ok();
    }

    [HttpDelete("remove/comments")]
    [Authorize(Roles = "Moderator, Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveComment(int id, CancellationToken cancellationToken = default)
    {
        await _gameServices.RemoveComment(id, cancellationToken);

        return Ok();
    }

    // PUT: api/games/update/{key}
    [HttpPut("update")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> Update([FromBody] GameDto game, CancellationToken cancellationToken = default)
    {
        await _gameServices.UpdateAsync(game, cancellationToken);

        return Ok(game);
    }

    [HttpPut("{key}/comments")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> AddCommentToGameAsync(string key, [FromBody] CommentDto comment, string userName, CancellationToken cancellationToken = default)
    {
        await _gameServices.AddCommentToGameAsync(key, comment, userName, cancellationToken);

        return Ok();
    }

    [HttpPut("addDeletedGameComment")]
    public async Task<ActionResult<CommentDto>> AddDeleteGameCommentAsync(string key, [FromBody] CommentDto comment, CancellationToken cancellationToken = default)
    {
        await _gameServices.AddCommentToDeletedGameAsync(key, comment, cancellationToken);

        return Ok();
    }

    [HttpPut("comments/reply")]
    [Authorize(Roles = "User, Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> ReplayToGameCommentAsync(int id, [FromBody] CommentDto comment, CancellationToken cancellationToken = default)
    {
        await _gameServices.ReplayToGameCommentAsync(id, comment, cancellationToken);

        return Ok();
    }

    [HttpPut("comments/quote")]
    [Authorize(Roles = "User, Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> QuoteToGameCommentAsync(int id, [FromBody] CommentDto comment, CancellationToken cancellationToken = default)
    {
        await _gameServices.QuoteToGameCommentAsync(id, comment, cancellationToken);

        return Ok();
    }

    [HttpPut("addImageToGame")]
    public async Task<ActionResult> AddImageToGameAsync(string key, string fileName, CancellationToken cancellationToken = default)
    {
        await _gameServices.AddImageToGameAsync(key, fileName, cancellationToken);

        return Ok();
    }
}