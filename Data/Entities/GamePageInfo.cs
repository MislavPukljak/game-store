namespace Data.SQL.Entities;

public class GamePageInfo
{
    public IEnumerable<Game> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int TotalItems { get; set; }

    public int PageSize { get; set; }
}
