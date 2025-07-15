using Data.SQL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.SQL.Data;

public class GameStoreContext : IdentityDbContext<User>
{
    public GameStoreContext(DbContextOptions<GameStoreContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Entities.Platform> Platforms { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<PaymentOption> PaymentOptions { get; set; }

    public DbSet<PlatformIBox> PlatformIBoxes { get; set; }

    public DbSet<Visa> Visas { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }

    public DbSet<Region> Regions { get; set; }

    public DbSet<Territory> Territories { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUserLogin<string>>().HasKey(p => p.UserId);

        builder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(b => b.Name)
            .IsRequired();

            entity.Property(p => p.Alias)
            .IsRequired();

            entity.HasIndex(u => u.Alias)
            .IsUnique();

            entity.HasMany(g => g.Genres)
            .WithMany(g => g.Games);

            entity.HasOne(g => g.Publishers)
            .WithMany(g => g.Games)
            .HasForeignKey(x => x.PublisherId);
        });

        builder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasMany(g => g.Games)
            .WithMany(g => g.Genres);

            entity.HasIndex(u => u.Name)
                .IsUnique();
        });

        builder.Entity<Entities.Platform>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(p => p.Type)
            .IsRequired();

            entity.HasIndex(u => u.Type)
                .IsUnique();
        });

        builder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(o => o.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(o => o.OrderId);
        });

        builder.Entity<PlatformIBox>()
            .HasNoKey();

        builder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(g => g.Game)
            .WithMany(g => g.Comments)
            .HasForeignKey(x => x.GameId);

            entity.HasOne(c => c.ParentComment)
            .WithMany(c => c.ChildComments)
            .HasForeignKey(x => x.ParentId);
        });

        builder.Entity<Cart>().HasKey(e => e.Id);

        builder.Entity<CartItem>().HasKey(e => e.Id);

        builder.Entity<Comment>().HasKey(e => e.Id);

        builder.Entity<Customer>().HasKey(e => e.Id);

        builder.Entity<Order>().HasKey(e => e.Id);

        builder.Entity<PaymentOption>().HasKey(e => e.Id);

        builder.Entity<Visa>().HasKey(e => e.Id);

        builder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(p => p.CompanyName)
            .IsUnique();
        });
    }
}
