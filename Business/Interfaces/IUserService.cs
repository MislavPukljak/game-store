using Business.DTO.Authorization;
using Data.SQL.Enums;

namespace Business.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();

    Task UpdateUserSqlAsync(string originalEmail, UserChangeDto userChangeDto);

    Task DeleteUserSqlAsync(string email);

    Task<AuthServiceResponseDto> RegisterAsync(UserChangeDto userDto);

    Task<AuthServiceResponseDto> ManageRoles(UpdatePermissionDto updatePermissionDto, UsersRoles.Roles role);
}
