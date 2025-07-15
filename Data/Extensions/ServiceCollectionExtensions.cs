using Data.SQL.Data;
using Data.SQL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data.SQL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
