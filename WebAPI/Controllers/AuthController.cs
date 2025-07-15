using Business.DTO.Authorization;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/authorization")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthServiceResponseDto>> Post([FromBody] AuthDto authDto)
    {
        var result = await _authService.LoginAsync(authDto);

        return Ok(result);
    }
}
