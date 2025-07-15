using System.Globalization;
using Azure.Storage.Blobs;
using Business.Interfaces;
using Business.Options;
using Business.Services;
using Data.MongoDb.Extensions;
using Data.SQL.Entities;
using Data.SQL.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPaymentOptionService, PaymentOptionService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IShipperService, ShipperService>();
        services.AddDataServices();
        services.AddMongoDataServices();
        services.AddAutoMapper();
        services.AddScoped<IAuthService, AuthorizationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserServiceClient, UserServiceClient>();
        services.AddScoped<IFileService, FileService>();

        bool useStub = configuration.GetValue<bool>("NotificationService:UseStub");

        if (useStub)
        {
            services.AddScoped<INotificationService, StubNotificationService>();
        }
        else
        {
            services.AddScoped<INotificationService, NotificationService>();
        }

        services.Configure<UserServiceClientOptions>(configuration.GetSection("UserServiceClient"));
        services.Configure<AzureBusOptions>(configuration.GetSection("AzureBus"));
        services.AddHttpClient();

        services.AddHttpContextAccessor();

        services.AddLocalization();

        services.Configure<RequestLocalizationOptions>(
            options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new("en-US"),
                    new("de-DE"),
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

        services.AddScoped(_ =>
        {
            return new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
        });

        services.Configure<ImagesData>(configuration.GetSection("ImagesData"));

        return services;
    }
}
