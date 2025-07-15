namespace Business.DTO;

public class GenreWithCountDto
{
    public int TotalCount { get; set; }

    public IEnumerable<GenreDto> Genres { get; set; }
}
