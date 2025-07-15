namespace Business.DTO;

public class PlatformWithCountDto
{
    public int TotalCount { get; set; }

    public IEnumerable<PlatformDto> Platforms { get; set; }
}
