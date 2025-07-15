using Business.DTO;

namespace Business.Interfaces;

public interface INotificationService
{
    Task SendOrderEmailAsync(OrderDto order, CancellationToken cancellationToken);

    Task SendOrderSmsAsync(OrderDto order, CancellationToken cancellationToken);

    Task SendOrderNotificationAsync(OrderDto order, CancellationToken cancellationToken);

    Task ChangeIsTurnedOffAsync(bool email, bool sms, bool notification, CancellationToken cancellationToken);
}
