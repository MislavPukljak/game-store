using Data.SQL.Interfaces;
using Data.SQL.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.SQL.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameStoreContext _context;

    public UnitOfWork(GameStoreContext context)
    {
        _context = context;
    }

    public IGameRepository GameRepository => new GameRepository(_context);

    public IGenreRepository GenreRepository => new GenreRepository(_context);

    public IPlatformRepository PlatformRepository => new PlatformRepository(_context);

    public IPublisherRepository PublisherRepository => new PublisherRepository(_context);

    public IOrderRepository OrderRepository => new OrderRepository(_context);

    public IOrderDetailRepository OrderDetailRepository => new OrderDetailRepository(_context);

    public ICustomerRepository CustomerRepository => new CustomerRepository(_context);

    public ICartRespoitory CartRepository => new CartRepository(_context);

    public IPaymentOptionRepository PaymentOptionRepository => new PaymentOptionRepository(_context);

    public IVisaRepository VisaRepository => new VisaRepository(_context);

    public ICommentRepository CommentRepository => new CommentRepository(_context);

    public IImagesDataRepository ImagesDataRepository => new ImagesDataRepository(_context);

    public INotificationRepository NotificationRepository => new NotificationRepository(_context);

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public Task SaveAsync()
    {
        return _context.SaveChangesAsync();
    }
}
