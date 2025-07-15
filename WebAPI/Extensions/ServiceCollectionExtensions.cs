using Business.Extensions;
using Data.SQL.Extensions;

namespace WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBusinessServices(configuration);
        services.AddDataServices();
        services.AddGameStoreContext(configuration);
        services.AddContext(configuration);

        return services;
    }
}
