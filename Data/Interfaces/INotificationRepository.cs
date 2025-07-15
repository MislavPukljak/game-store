using Data.SQL.Entities;

namespace Data.SQL.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<Notification> GetNotification(string queueName, CancellationToken cancellationToken);
}
