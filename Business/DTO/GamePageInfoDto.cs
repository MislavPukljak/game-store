namespace Business.DTO;

public class GamePageInfoDto
{
    public IEnumerable<GameDto> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int TotalItems { get; set; }

    public int PageSize { get; set; }
}
