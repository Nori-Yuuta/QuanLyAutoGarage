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

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique();
        mb.Entity<Order>()
          .HasMany(o => o.Items).WithOne(i => i.Order!)
          .HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Order>().Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        mb.Entity<Order>().Property(o => o.Shipping).HasColumnType("decimal(18,2)");
        mb.Entity<Order>().Property(o => o.Discount).HasColumnType("decimal(18,2)");
        mb.Entity<Order>().Property(o => o.Total).HasColumnType("decimal(18,2)");
        mb.Entity<OrderItem>().Property(i => i.Price).HasColumnType("decimal(18,2)");
    }
}
