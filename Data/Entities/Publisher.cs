namespace Data.SQL.Entities;

public class Publisher
{
    public int Id { get; set; }

    public string CompanyName { get; set; }

    public string Description { get; set; }

    public string HomePage { get; set; }

    public List<Game> Games { get; set; }
}
