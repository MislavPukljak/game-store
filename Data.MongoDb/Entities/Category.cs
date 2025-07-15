using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("CategoryID")]
    public int CategoryId { get; set; }

    [BsonElement("CategoryName")]
    public string CategoryName { get; set; }

    [BsonElement("Description")]
    public string Description { get; set; }

    [BsonElement("Picture")]
    public byte[]? Picture { get; set; }
}
