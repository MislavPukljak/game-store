using Business.DTO;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/publishers")]
[ApiController]
[Authorize(Roles = "Administrator, Manager")]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherServices;

    public PublisherController(IPublisherService publisherServices)
    {
        _publisherServices = publisherServices;
    }

    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PublisherDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublisherDto>> AddAsync([FromBody] PublisherDto publisher, CancellationToken cancellationToken = default)
    {
        await _publisherServices.AddAsync(publisher, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = publisher.Id }, publisher);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherWithCountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var publishers = await _publisherServices.GetAllAsync(cancellationToken);

        return Ok(publishers);
    }

    [HttpGet("{companyName}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PublisherDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> GetByCompanyNameAsync(string companyName, CancellationToken cancellationToken = default)
    {
        var publisher = await _publisherServices.GetByCompanyName(companyName, cancellationToken);

        return Ok(publisher);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PublisherDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var game = await _publisherServices.GetPublisherByIdAsync(id, cancellationToken);

        return Ok(game);
    }

    [HttpGet("game/{key}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> GetPublisherByGameKey(string key, CancellationToken cancellationToken = default)
    {
        var game = await _publisherServices.GetPublisherByGameKey(key, cancellationToken);

        return Ok(game);
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PublisherDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync([FromBody] PublisherDto publisherDto, CancellationToken cancellationToken = default)
    {
        await _publisherServices.UpdateAsync(publisherDto, cancellationToken);

        return Ok();
    }

    [HttpDelete("remove/{companyName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string companyName, CancellationToken cancellationToken = default)
    {
        await _publisherServices.DeleteAsync(companyName, cancellationToken);

        return Ok();
    }
}
