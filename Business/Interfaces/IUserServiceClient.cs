using Business.DTO.Authorization;

namespace Business.Interfaces;

public interface IUserServiceClient
{
    Task<List<UserDto>> GetUsersAsync();

    Task<AuthServiceResponseDto> PostUserAsync(UserChangeDto userChangeDto);

    Task<AuthServiceResponseDto> PutUserAsync(string originalEmail, UserChangeDto userChangeDto);

    Task<AuthServiceResponseDto> DeleteUserAsync(string email);

    Task<UserDto> PostAuthAsync(AuthDto authDto);
}
