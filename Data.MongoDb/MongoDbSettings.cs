namespace Data.MongoDb;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ProductCollectionName { get; set; } = null!;

    public string OrderCollectionName { get; set; } = null!;

    public string LoggerCollectionName { get; set; } = null!;

    public string SupplierCollectionName { get; set; } = null!;

    public string CategoryCollectionName { get; set; } = null!;

    public string ShipperCollectionName { get; set; } = null!;

    public string OrderDetailCollectionName { get; set; } = null!;

    public string CustomerCollectionName { get; set; } = null!;

    public string EmployeeCollectionName { get; set; } = null!;

    public string EmployeeTerritoryCollectionName { get; set; } = null!;

    public string RegionCollectionName { get; set; } = null!;

    public string TerritoryCollectionName { get; set; } = null!;
}
