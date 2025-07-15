using MongoDB.Driver;

namespace Data.MongoDb.Interfaces;

public interface IMongoContext
{
    Task<int> SaveChanges();

    IMongoCollection<T> GetCollection<T>(string name);
}
