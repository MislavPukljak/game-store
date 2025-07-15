using Business.DTO.Authorization;
using Business.Interfaces;
using Data.SQL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("allUsers")]
    public async Task<IActionResult> GetAllUser()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost("newUser")]
    public async Task<IActionResult> PostUser([FromBody] UserChangeDto userDto)
    {
        await _userService.RegisterAsync(userDto);

        return Ok(userDto);
    }

    [HttpPut("{originalEmail}")]
    public async Task<IActionResult> PutUser(string originalEmail, [FromBody] UserChangeDto userDto)
    {
        await _userService.UpdateUserSqlAsync(originalEmail, userDto);
        return Ok(userDto);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUsers([FromBody] string email)
    {
        await _userService.DeleteUserSqlAsync(email);
        return Ok();
    }

    [HttpPost("manageRoles")]
    public async Task<IActionResult> ManageRoles([FromBody] UpdatePermissionDto updatePermissionDto, [FromQuery] UsersRoles.Roles role)
    {
        var result = await _userService.ManageRoles(updatePermissionDto, role);

        return Ok(result);
    }
}
