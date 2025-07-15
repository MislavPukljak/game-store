using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/genres")]
[ApiController]
[Authorize(Roles = "Administrator, Manager")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreServices;

    public GenreController(IGenreService genreServices)
    {
        _genreServices = genreServices;
    }

    // POST: api/genres/new
    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlatformDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenreDto>> AddAsync([FromBody] GenreDto genre, CancellationToken cancellationToken = default)
    {
        await _genreServices.CreateGenreAsync(genre, cancellationToken);

        return CreatedAtAction(nameof(GetGenreByIdAsync), new { id = genre.Id }, genre);
    }

    // GET: api/genres
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GenreWithCountDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var genres = await _genreServices.GetAllGenresAsync(cancellationToken);

        return Ok(genres);
    }

    // GET: api/genres/{id}
    [HttpGet("{id:int}", Name = "GetGenreByIdAsync")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenreDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> GetGenreByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var genre = await _genreServices.GetGenreByIdAsync(id, cancellationToken);

        return Ok(genre);
    }

    [HttpGet("{key}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenresByGameKey(string key, CancellationToken cancellationToken = default)
    {
        var games = await _genreServices.GetGenresByGameKey(key, cancellationToken);

        return Ok(games);
    }

    // PUT: api/genres/update
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenreDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> Update([FromBody] GenreDto genreDto, CancellationToken cancellationToken = default)
    {
        await _genreServices.UpdateGenreAsync(genreDto, cancellationToken);

        return Ok(genreDto);
    }

    // DELETE: api/genres/remove
    [HttpDelete("remove")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string categoryName, CancellationToken cancellationToken = default)
    {
        await _genreServices.DeleteGenreAsync(categoryName, cancellationToken);

        return NoContent();
    }
}
