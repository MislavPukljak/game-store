namespace Business.DTO;

public class PublisherWithCountDto
{
    public int TotalCount { get; set; }

    public IEnumerable<PublisherDto> Publishers { get; set; }
}
