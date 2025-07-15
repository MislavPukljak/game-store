using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/platforms")]
[ApiController]
[Authorize(Roles = "Administrator, Manager")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService _platformServices;

    public PlatformController(IPlatformService platformServices)
    {
        _platformServices = platformServices;
    }

    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlatformDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlatformDto>> AddAsync([FromBody] PlatformDto platform, CancellationToken cancellationToken = default)
    {
        await _platformServices.CreatePlatformAsync(platform, cancellationToken);

        return CreatedAtAction(nameof(GetPlatformByIdAsync), new { id = platform.Id }, platform);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlatformDto>> GetAllPlatformAsync(CancellationToken cancellationToken = default)
    {
        var platforms = await _platformServices.GetAllPlatformsAsync(cancellationToken);

        return Ok(platforms);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlatformDto>> GetPlatformByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var platform = await _platformServices.GetPlatformByIdAsync(id, cancellationToken);

        return Ok(platform);
    }

    [HttpGet("{key}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PlatformDto>>> GetPlatformsByGameKey(string key, CancellationToken cancellationToken = default)
    {
        var games = await _platformServices.GetPlatformsByGameKey(key, cancellationToken);

        return Ok(games);
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlatformDto>> UpdateAsync([FromBody] PlatformDto platform, CancellationToken cancellationToken = default)
    {
        await _platformServices.UpdatePlatformAsync(platform, cancellationToken);

        return Ok(platform);
    }

    [HttpDelete("remove/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _platformServices.DeletePlatformAsync(id, cancellationToken);

        return Ok();
    }
}
