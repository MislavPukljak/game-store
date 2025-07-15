namespace Business.DTO;

public class GameDto
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public short UnitInStock { get; set; }

    public bool Discontinued { get; set; }

    public List<PlatformDto> Platforms { get; set; }

    public int? GenreId { get; set; }

    public List<GenreDto>? Genres { get; set; }

    public int? PublisherId { get; set; }

    public PublisherDto? Publishers { get; set; }

    public List<CommentDto> Comments { get; set; }

    public DateTime PublishedDate { get; set; }

    public int Views { get; set; }

    public List<ImageDataDto>? Images { get; set; }
}
