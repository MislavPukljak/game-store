using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Entities;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("ProductID")]
    public int ProductID { get; set; }

    [BsonElement("ProductName")]
    public string ProductName { get; set; }

    [BsonElement("Alias")]
    public string Alias { get; set; }

    [BsonElement("SupplierID")]
    public int SupplierID { get; set; }

    [BsonElement("CategoryID")]
    public int CategoryID { get; set; }

    [BsonElement("QuantityPerUnit")]
    public string QuantityPerUnit { get; set; }

    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("UnitsInStock")]
    public int UnitsInStock { get; set; }

    [BsonElement("UnitsOnOrder")]
    public int UnitsOnOrder { get; set; }

    [BsonElement("ReorderLevel")]
    public int ReorderLevel { get; set; }

    [BsonElement("Discontinued")]
    public bool Discontinued { get; set; }
}
