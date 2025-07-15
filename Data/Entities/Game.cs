namespace Data.SQL.Entities;

public class Game
{
    public int Id { get; set; }

    public string Alias { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public short UnitInStock { get; set; }

    public bool Discontinued { get; set; }

    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }

    public int PublisherId { get; set; }

    public Publisher Publishers { get; set; }

    public List<Comment> Comments { get; set; }

    public DateTime PublishedDate { get; set; }

    public int Views { get; set; }

    public bool IsDeleted { get; set; }

    public List<ImagesData>? Images { get; set; }
}
