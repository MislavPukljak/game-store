using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Business.Resources;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class PlatformService : IPlatformService
{
    private const string AllPlatform = "allPlatforms";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PlatformService> _logger;
    private readonly IStringLocalizer<Resource> _localizer;

    public PlatformService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<PlatformService> logger, IStringLocalizer<Resource> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task CreatePlatformAsync(PlatformDto platform, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding a new platform to the database");

        var platformMapped = _mapper.Map<Data.SQL.Entities.Platform>(platform);

        await _unitOfWork.PlatformRepository.AddAsync(platformMapped, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Platform has been added to the database");
    }

    public async Task DeletePlatformAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting platform with id: {id}", id);

        var platform = await _unitOfWork.PlatformRepository.GetByIdAsync(id, cancellationToken);

        if (platform is not null)
        {
            _unitOfWork.PlatformRepository.DeleteAsync(platform, cancellationToken);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Platform has been deleted from the database");

            return;
        }

        _logger.LogError("Platform hasn't been found in db.");

        throw new PlatformException($"Platform with id {id} not found");
    }

    public async Task<PlatformWithCountDto> GetAllPlatformsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all platforms from the database");

        var cachedGenre = _memoryCache.Get<IEnumerable<PlatformDto>>(AllPlatform);

        if (cachedGenre is not null)
        {
            var cachedGenreCount = new PlatformWithCountDto
            {
                TotalCount = cachedGenre.Count(),
                Platforms = cachedGenre,
            };

            return cachedGenreCount;
        }

        var platforms = await _unitOfWork.PlatformRepository.GetAllAsync(cancellationToken);
        var platformsDto = _mapper.Map<IEnumerable<PlatformDto>>(platforms);

        foreach (var platform in platformsDto)
        {
            await LocalizeAsync(platform);
        }

        var platformWithCount = new PlatformWithCountDto
        {
            TotalCount = platformsDto.Count(),
            Platforms = platformsDto,
        };

        _memoryCache.Set(AllPlatform, platformsDto, TimeSpan.FromSeconds(3));

        return platformWithCount;
    }

    public async Task<PlatformDto> GetPlatformByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting platform by id from the database");

        var cachedGenre = _memoryCache.Get<PlatformDto>(id);

        if (cachedGenre is not null)
        {
            return cachedGenre;
        }

        var platform = await _unitOfWork.PlatformRepository.GetByIdAsync(id, cancellationToken);

        if (platform is not null)
        {
            var platformDto = _mapper.Map<PlatformDto>(platform);

            await LocalizeAsync(platformDto);

            _memoryCache.Set(id, platformDto, TimeSpan.FromSeconds(3));
            return platformDto;
        }

        _logger.LogError("Platform hasn't been found in db.");

        throw new PlatformException($"Platform with id {id} not found");
    }

    public async Task<IEnumerable<PlatformDto>> GetPlatformsByGameKey(string key, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all platforms from the database by game key.");

            var games = await _unitOfWork.PlatformRepository.GetPlatformsByGameKey(key, CancellationToken.None);

            if (games is not null)
            {
                var gameModels = _mapper.Map<IEnumerable<PlatformDto>>(games);

                foreach (var platform in gameModels)
                {
                    await LocalizeAsync(platform);
                }

                return gameModels;
            }

            throw new GameException("Games not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Game not found {ex}", ex);
        }

        throw new GameException("Games not found.");
    }

    public async Task UpdatePlatformAsync(PlatformDto platformDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating platform by id from the database");
        var platform = await _unitOfWork.PlatformRepository.GetByIdAsync(platformDto.Id, cancellationToken);

        if (platform is not null)
        {
            var createdPlatform = _mapper.Map<Data.SQL.Entities.Platform>(platformDto);

            _unitOfWork.PlatformRepository.Update(createdPlatform);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Platform has been updated in the database");

            return;
        }

        _logger.LogError("Platform hasn't been found in db.");

        throw new PlatformException($"Platform with id {platformDto.Id} not found");
    }

    private async Task<PlatformDto> LocalizeAsync(PlatformDto platform)
    {
        platform.Type = _localizer[platform.Type].Value;

        return await Task.FromResult(platform);
    }
}
