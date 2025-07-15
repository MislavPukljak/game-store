using Business.DTO;

namespace Business.Interfaces;

public interface IPlatformService
{
    Task<PlatformWithCountDto> GetAllPlatformsAsync(CancellationToken cancellationToken);

    Task<PlatformDto> GetPlatformByIdAsync(int id, CancellationToken cancellationToken);

    Task CreatePlatformAsync(PlatformDto platform, CancellationToken cancellationToken);

    Task UpdatePlatformAsync(PlatformDto platformDto, CancellationToken cancellationToken);

    Task DeletePlatformAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<PlatformDto>> GetPlatformsByGameKey(string key, CancellationToken cancellationToken);
}
