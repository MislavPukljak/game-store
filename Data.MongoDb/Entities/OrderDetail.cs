using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class OrderDetail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("OrderID")]
    public int OrderID { get; set; }

    [BsonElement("ProductID")]
    public int ProductID { get; set; }

    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("Quantity")]
    public int Quantity { get; set; }

    [BsonElement("Discount")]
    public decimal Discount { get; set; }
}
