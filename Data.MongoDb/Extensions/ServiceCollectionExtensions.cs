using Data.MongoDb.Context;
using Data.MongoDb.Data;
using Data.MongoDb.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data.MongoDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDataServices(this IServiceCollection services)
    {
        services.AddScoped<INoSqlUnitOfWork, NoSqlUnitOfWork>();
        services.AddScoped<IMongoContext, MongoContext>();

        return services;
    }
}
