using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class ProductPageInfo
{
    [BsonElement("Products")]
    public IEnumerable<Product> Products { get; set; }

    [BsonElement("TotalPages")]
    public int TotalPages { get; set; }

    [BsonElement("CurrentPage")]
    public int CurrentPage { get; set; }

    [BsonElement("TotalItems")]
    public int TotalItems { get; set; }

    [BsonElement("PageSize")]
    public int PageSize { get; set; }
}
