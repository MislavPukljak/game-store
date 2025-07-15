using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class EmployeeTerritory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("EmployeeID")]
    public int EmployeeID { get; set; }

    [BsonElement("TerritoryID")]
    public string TerritoryID { get; set; }
}
