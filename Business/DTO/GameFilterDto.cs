using Data.SQL.Enums;

namespace Business.DTO;

public class GameFilterDto
{
    public List<string>? Genres { get; set; }

    public List<string>? Platforms { get; set; }

    public string? Publishers { get; set; }

    public decimal? PriceFrom { get; set; }

    public decimal? PriceTo { get; set; }

    public string? NameStart { get; set; }

    public Time.TimeRange? PublishedDate { get; set; }

    public OrderFilter.OrderBy? Sort { get; set; }

    public int CurrentPage { get; set; } = 1;

    public PageInfo.PerPage ItemsPerPage { get; set; } = PageInfo.PerPage.Ten;
}
