using System.Text;
using System.Text.Json;
using Business.DTO;
using Business.Options;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Business.Services;

public class AzureBusService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly string _connectionString;

    public AzureBusService(
        ILogger<NotificationService> logger,
        IOptions<AzureBusOptions> options)
    {
        _logger = logger;
        _connectionString = options.Value.AzureServiceBusConnection;
    }

    public async Task SendMessageAsync(OrderDto order, NotificationDto queueClient)
    {
        try
        {
            var connectionString = _connectionString;

            var qclient = new QueueClient(connectionString, queueClient.Queue);

            var message = new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order)))
            {
                Label = $"New order ID:{order.Id}",
            };

            await qclient.SendAsync(message);
            await qclient.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception thrown in SendMessagesAsync: {ex}");
        }
    }
}
