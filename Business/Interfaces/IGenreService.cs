using Business.DTO;

namespace Business.Interfaces;

public interface IGenreService
{
    Task<GenreWithCountDto> GetAllGenresAsync(CancellationToken cancellationToken);

    Task<GenreDto> GetGenreByIdAsync(int id, CancellationToken cancellationToken);

    Task CreateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);

    Task<GenreDto> UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);

    Task DeleteGenreAsync(string name, CancellationToken cancellationToken);

    Task<IEnumerable<GenreDto>> GetGenresByGameKey(string key, CancellationToken cancellationToken);
}
