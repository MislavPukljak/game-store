using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/shippers")]
[ApiController]
[Authorize(Roles = "Administrator, Manager")]
public class ShipperController : ControllerBase
{
    private readonly IShipperService _shipperService;

    public ShipperController(IShipperService shipperService)
    {
        _shipperService = shipperService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var shippers = await _shipperService.GetAllAsync(cancellationToken);
        return Ok(shippers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var shipper = await _shipperService.GetByIdAsync(id, cancellationToken);
        return Ok(shipper);
    }
}
