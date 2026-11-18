namespace AutoGarageMVC.Models;

using AutoGarageMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class AutoGarageDbContext : DbContext
{
    public AutoGarageDbContext(DbContextOptions<AutoGarageDbContext> o) : base(o) { }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Accessory> Accessories => Set<Accessory>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<News> News => Set<News>();
    public DbSet<TestDriveBooking> TestDriveBookings => Set<TestDriveBooking>();
    public DbSet<ServiceAppointment> ServiceAppointments => Set<ServiceAppointment>();
    public DbSet<TradeInValuation> TradeInValuations => Set<TradeInValuation>();
    public DbSet<ServiceHistoryRecord> ServiceHistoryRecords => Set<ServiceHistoryRecord>();
    public DbSet<CustomerLead> CustomerLeads => Set<CustomerLead>();
    public DbSet<CarDepositBooking> CarDepositBookings => Set<CarDepositBooking>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique();
        mb.Entity<ChatMessage>().HasIndex(m => m.SessionId);

        // User - Orders (1 - N)
        mb.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User!)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Order - OrderItems (1 - N)
        mb.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order!)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Accessory - OrderItems (1 - N)
        mb.Entity<Accessory>()
            .HasMany(a => a.OrderItems)
            .WithOne(i => i.Accessory!)
            .HasForeignKey(i => i.AccessoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // User - ChatMessages (1 - N)
        mb.Entity<User>()
            .HasMany(u => u.ChatMessages)
            .WithOne(m => m.User!)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // User - News (1 - N Author)
        mb.Entity<User>()
            .HasMany(u => u.NewsArticles)
            .WithOne(n => n.Author!)
            .HasForeignKey(n => n.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        // User - Notifications (1 - N)
        mb.Entity<User>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.User!)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Precision configurations
        mb.Entity<Order>().Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        mb.Entity<Order>().Property(o => o.Shipping).HasColumnType("decimal(18,2)");
        mb.Entity<Order>().Property(o => o.Discount).HasColumnType("decimal(18,2)");
        mb.Entity<Order>().Property(o => o.Total).HasColumnType("decimal(18,2)");
        mb.Entity<OrderItem>().Property(i => i.Price).HasColumnType("decimal(18,2)");
    }
}
