using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class Region
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("RegionID")]
    public int RegionId { get; set; }

    [BsonElement("RegionDescription")]
    public string RegionDescription { get; set; }
}
