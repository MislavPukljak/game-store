using Data.MongoDb.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Data.MongoDb.Context;
public class MongoContext : IMongoContext
{
    private readonly List<Func<Task>> _commands;

    private readonly IOptions<MongoDbSettings> _configuration;

    public MongoContext(IOptions<MongoDbSettings> configuration)
    {
        _configuration = configuration;

        _commands = new List<Func<Task>>();
    }

    public IClientSessionHandle Session { get; set; }

    public MongoClient MongoClient { get; set; }

    private IMongoDatabase Database { get; set; }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        ConfigureMongo();

        return Database.GetCollection<T>(name);
    }

    public async Task<int> SaveChanges()
    {
        ConfigureMongo();

        var commandTasks = _commands.Select(c => c());

        await Task.WhenAll(commandTasks);

        return _commands.Count;
    }

    private void ConfigureMongo()
    {
        if (MongoClient != null)
        {
            return;
        }

        MongoClient = new MongoClient(_configuration.Value.ConnectionString);

        Database = MongoClient.GetDatabase(_configuration.Value.DatabaseName);
    }
}
