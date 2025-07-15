namespace Data.SQL.Entities;

public class Platform
{
    public int Id { get; set; }

    public string Type { get; set; }

    public List<Game> Games { get; set; }
}