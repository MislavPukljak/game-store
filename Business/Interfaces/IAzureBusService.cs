using Business.DTO;

namespace Business.Interfaces;

public interface IAzureBusService
{
    Task SendMessageAsync(OrderDto order, NotificationDto queueClient);
}
