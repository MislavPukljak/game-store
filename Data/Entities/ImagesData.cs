namespace Data.SQL.Entities;

public class ImagesData
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Container { get; set; }

    public string ContentType { get; set; }

    public List<Game> Games { get; set; }
}
