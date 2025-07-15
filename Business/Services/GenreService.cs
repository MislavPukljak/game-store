using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Business.Resources;
using Data.MongoDb.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class GenreService : IGenreService
{
    private const string AllGenre = "allGenre";

    private readonly IUnitOfWork _unitOfWork;
    private readonly INoSqlUnitOfWork _noSqlUnitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<GenreService> _logger;
    private readonly IStringLocalizer<Resource> _localizer;

    public GenreService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<GenreService> logger, INoSqlUnitOfWork noSqlUnitOfWork, IStringLocalizer<Resource> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _logger = logger;
        _noSqlUnitOfWork = noSqlUnitOfWork;
        _localizer = localizer;
    }

    public async Task CreateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding a new genre to the database");

        var genre = new Genre
        {
            Name = genreDto.Name,
            Subcategory = genreDto.Subcategory,
        };

        await _unitOfWork.GenreRepository.AddAsync(genre, cancellationToken);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Genre added to the database");
    }

    public async Task DeleteGenreAsync(string name, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting genre by id from the database");

        var genre = await _unitOfWork.GenreRepository.GetByCategoryName(name, cancellationToken);
        if (genre is not null)
        {
            await DeleteGenreSqlAsync(name, cancellationToken);
            return;
        }

        var category = await _noSqlUnitOfWork.CategoryRepository.GetByCategoryNameAsync(name, cancellationToken);
        if (category is not null)
        {
            await DeleteCategoryAsync(name, cancellationToken);
            return;
        }

        _logger.LogError("Genre with name: {name}, hasn't been found in db.", name);

        throw new GenreException($"Genre with name {name} not found");
    }

    public async Task<GenreWithCountDto> GetAllGenresAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all genres from the database");

        var cachedGenre = _memoryCache.Get<IEnumerable<GenreDto>>(AllGenre);

        if (cachedGenre is not null)
        {
            var cachedGenreCount = new GenreWithCountDto
            {
                TotalCount = cachedGenre.Count(),
                Genres = cachedGenre,
            };

            return cachedGenreCount;
        }

        var genres = await _unitOfWork.GenreRepository.GetAllAsync(cancellationToken);
        var categories = await _noSqlUnitOfWork.CategoryRepository.GetAllAsync(cancellationToken);

        var genresDto = _mapper.Map<IEnumerable<GenreDto>>(genres);
        var categoriesDto = _mapper.Map<IEnumerable<GenreDto>>(categories);

        var genresCategory = genresDto.Concat(categoriesDto);

        foreach (var genre in genresDto)
        {
            await LocalizeAsync(genre);
        }

        _memoryCache.Set(AllGenre, genresDto, TimeSpan.FromSeconds(3));

        var genresWithCount = new GenreWithCountDto
        {
            TotalCount = genresCategory.Count(),
            Genres = genresCategory,
        };
        return genresWithCount;
    }

    public async Task<GenreDto> GetGenreByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting genre by id from the database");

        var cachedGenre = _memoryCache.Get<GenreDto>(id);

        if (cachedGenre is not null)
        {
            return cachedGenre;
        }

        var genre = await _unitOfWork.GenreRepository.GetByIdAsync(id, cancellationToken);
        var category = await _noSqlUnitOfWork.CategoryRepository.GetByCategoryId(id, cancellationToken);

        if (genre is not null)
        {
            var genreEntity = _mapper.Map<GenreDto>(genre);

            await LocalizeAsync(genreEntity);

            _memoryCache.Set(id, genreEntity, TimeSpan.FromSeconds(3));
            return genreEntity;
        }
        else if (category is not null)
        {
            var categoryEntity = _mapper.Map<GenreDto>(category);

            return categoryEntity;
        }

        _logger.LogError("Genre with id: {id}, hasn't been found in db.", id);

        throw new GenreException($"Genre with id {id} not found");
    }

    public async Task<IEnumerable<GenreDto>> GetGenresByGameKey(string key, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all genres from the database by game key.");

            var games = await _unitOfWork.GenreRepository.GetGenresByGameKey(key, CancellationToken.None);

            if (games is not null)
            {
                var gameModels = _mapper.Map<IEnumerable<GenreDto>>(games);

                foreach (var genre in gameModels)
                {
                    await LocalizeAsync(genre);
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

    public async Task<GenreDto> UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating genre by id from the database");

        var genre = await UpdateSqlAsync(genreDto, cancellationToken);
        if (genre is not null)
        {
            return genre;
        }

        var category = await UpdateMongoAsync(genreDto, cancellationToken);
        if (category is not null)
        {
            return category;
        }

        _logger.LogError("Genre with id: {id}, hasn't been found in db.", genreDto.Id);

        throw new GenreException($"Genre with id {genreDto.Id} not found");
    }

    private async Task DeleteGenreSqlAsync(string name, CancellationToken cancellationToken)
    {
        var genre = await _unitOfWork.GenreRepository.GetByCategoryName(name, cancellationToken);
        _unitOfWork.GenreRepository.DeleteAsync(genre, cancellationToken);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Genre: {genre} deleted from the SQL", genre);
    }

    private async Task DeleteCategoryAsync(string name, CancellationToken cancellationToken)
    {
        var category = await _noSqlUnitOfWork.CategoryRepository.GetByCategoryNameAsync(name, cancellationToken);

        await _noSqlUnitOfWork.CategoryRepository.DeleteCategoryAsync(name, cancellationToken);

        _logger.LogInformation("Category: {category} deleted from the MongoDb", category);
    }

    private async Task<GenreDto> UpdateSqlAsync(GenreDto genre, CancellationToken cancellationToken)
    {
        var genreEntity = await _unitOfWork.GenreRepository.GetByCategoryName(genre.Name, cancellationToken);

        if (genreEntity is not null)
        {
            var genreModel = _mapper.Map<Genre>(genre);

            _unitOfWork.GenreRepository.Update(genreModel);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Genre: {genre} updated in the SQL", genre);

            return _mapper.Map<GenreDto>(genreEntity);
        }

        return null;
    }

    private async Task<GenreDto> UpdateMongoAsync(GenreDto genre, CancellationToken cancellationToken)
    {
        var category = await _noSqlUnitOfWork.CategoryRepository.GetByCategoryNameAsync(genre.Name, cancellationToken);

        if (category is not null)
        {
            var genreModel = _mapper.Map<Genre>(genre);

            await _unitOfWork.GenreRepository.AddAsync(genreModel, cancellationToken);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Category: {category} updated in the MongoDb", category);

            return _mapper.Map<GenreDto>(category);
        }

        return null;
    }

    private async Task<GenreDto> LocalizeAsync(GenreDto genre)
    {
        genre.Name = _localizer[genre.Name].Value;
        genre.Subcategory = _localizer[genre.Subcategory].Value;

        return await Task.FromResult(genre);
    }
}
