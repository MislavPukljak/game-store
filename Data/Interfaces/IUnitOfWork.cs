using Microsoft.EntityFrameworkCore.Storage;

namespace Data.SQL.Interfaces;

public interface IUnitOfWork
{
    IGameRepository GameRepository { get; }

    IGenreRepository GenreRepository { get; }

    IPlatformRepository PlatformRepository { get; }

    IPublisherRepository PublisherRepository { get; }

    IOrderRepository OrderRepository { get; }

    IOrderDetailRepository OrderDetailRepository { get; }

    ICustomerRepository CustomerRepository { get; }

    ICartRespoitory CartRepository { get; }

    IPaymentOptionRepository PaymentOptionRepository { get; }

    IVisaRepository VisaRepository { get; }

    ICommentRepository CommentRepository { get; }

    IImagesDataRepository ImagesDataRepository { get; }

    INotificationRepository NotificationRepository { get; }

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task SaveAsync();
}
