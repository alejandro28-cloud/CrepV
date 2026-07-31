using Microsoft.EntityFrameworkCore;
using CafeCreperiaApi.Models;

namespace CafeCreperiaApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Apertura> Aperturas => Set<Apertura>();
    public DbSet<Corte> Cortes => Set<Corte>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Orders> Orders => Set<Orders>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<User>().ToTable("users");
        mb.Entity<Apertura>().ToTable("aperturas");
        mb.Entity<Corte>().ToTable("cortes");
        mb.Entity<Product>().ToTable("products");
        mb.Entity<Orders>().ToTable("orders");
        mb.Entity<OrderItem>().ToTable("order_items");

        // Enums stored as strings for readability
        mb.Entity<User>().Property(u => u.Role).HasConversion<string>();
        mb.Entity<Apertura>().Property(a => a.Status).HasConversion<string>();
        mb.Entity<Product>().Property(p => p.Category).HasConversion<string>();
        mb.Entity<Product>().Property(p => p.Department).HasConversion<string>();
        mb.Entity<Orders>().Property(o => o.ConsumeType).HasConversion<string>();
        mb.Entity<Orders>().Property(o => o.PaymentMethod).HasConversion<string>();
        mb.Entity<Orders>().Property(o => o.Status).HasConversion<string>();
        mb.Entity<OrderItem>().Property(o => o.Department).HasConversion<string>();

        mb.Entity<Corte>()
        .HasOne(c => c.ClosedByUser)
        .WithMany()
        .HasForeignKey(c => c.ClosedBy)
        .OnDelete(DeleteBehavior.NoAction);

        mb.Entity<Apertura>()
            .HasOne(a => a.OpenedByUser)
            .WithMany(u => u.Aperturas)
            .HasForeignKey(a => a.OpenedBy)
            .OnDelete(DeleteBehavior.NoAction);

        mb.Entity<Orders>()
            .HasOne(o => o.CreatedByUser)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        mb.Entity<Orders>()
            .HasOne(o => o.Apertura)
            .WithMany(a => a.Orders)
            .HasForeignKey(o => o.AperturaId)
            .OnDelete(DeleteBehavior.NoAction);

        mb.Entity<Corte>()
            .HasOne(c => c.Apertura)
            .WithOne(a => a.Corte)
            .HasForeignKey<Corte>(c => c.AperturaId)
            .OnDelete(DeleteBehavior.NoAction);

        mb.Entity<OrderItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique: only one active apertura per department
        mb.Entity<Apertura>()
            .HasIndex(a => new { a.Status })
            .HasFilter("\"Status\" = 'open'")
            .IsUnique();

        // ── Seed data ──────────────────────────────────────────────────────
        mb.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                // Password: admin123
                PasswordHash = "$2a$11$Jvy3l91RFVq5528881is0upgH6fhMVb4Q4Ogq1Jx8U9Y51RkLiaq2",
                Role = UserRole.admin,
            },
            new User
            {
                Id = 2,
                Username = "victor",
                // Password: admin123
                PasswordHash = "$2a$11$6ehw4aXWqHdw6yFAkbBgEOBckKOcjK/4cVUeKZiQQX7SE0LlP6u8C",
                Role = UserRole.admin,
            },
            new User
            {
                Id = 3,
                Username = "seller",
                // Password: seller123
                PasswordHash = "$2a$11$Ty4.UhT3RQJW1i.pOGFmCORnOTCi4allN94lww/DjmsO3p45ATKTa",
                Role = UserRole.seller,
            }
        );

        mb.Entity<Product>().HasData(
            // Tienda — precio variable, lo establece el vendedor en cada venta
            new Product { Id = 17, Name = "Tienda", Price = 0, Category = Category.extras, Department = Department.tienda, Available = true }
        );
    }
}
