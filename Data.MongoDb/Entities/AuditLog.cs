using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class AuditLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("EntityType")]
    public string EntityType { get; set; }

    [BsonElement("EventDate")]
    public DateTime EventDate { get; set; }

    [BsonElement("Action")]
    public string Action { get; set; }

    [BsonElement("OldObject")]
    public string OldObject { get; set; }

    [BsonElement("NewObject")]
    public string NewObject { get; set; }
}
