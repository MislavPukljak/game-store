using Data.MongoDb.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class ProductFilter
{
    [BsonElement("Category")]
    public List<string?> Category { get; set; }

    [BsonElement("Supplier")]
    public string? Supplier { get; set; }

    [BsonElement("PriceFrom")]
    public decimal? PriceFrom { get; set; }

    [BsonElement("PriceTo")]
    public decimal? PriceTo { get; set; }

    [BsonElement("NameStart")]
    public string? NameStart { get; set; }

    [BsonElement("Sort")]
    public OrderFilter.OrderBy? Sort { get; set; }

    [BsonElement("CurrentPage")]
    public int CurrentPage { get; set; } = 1;

    [BsonElement("ItemsPerPage")]
    public PageInfo.PerPage ItemsPerPage { get; set; } = PageInfo.PerPage.Ten;
}
