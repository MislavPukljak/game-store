namespace Business.DTO;

public class GameWithCountDto
{
    public int TotalCount { get; set; }

    public IEnumerable<GameDto> Games { get; set; }
}
