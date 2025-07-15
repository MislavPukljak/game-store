using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class Territory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("TerritoryID")]
    public int TerritoryId { get; set; }

    [BsonElement("TerritoryDescription")]
    public string TerritoryDescription { get; set; }

    [BsonElement("RegionID")]
    public int RegionId { get; set; }
}
