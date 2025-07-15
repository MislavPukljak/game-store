using Data.SQL.Data;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Repository;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private readonly GameStoreContext _context;

    public NotificationRepository(GameStoreContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Notification> GetNotification(string queueName, CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(x => x.Queue == queueName, cancellationToken);
    }
}
