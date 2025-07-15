using Business.DTO.Authorization;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResponseDto> LoginAsync(AuthDto authDto);
}