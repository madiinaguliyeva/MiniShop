using Microsoft.EntityFrameworkCore;
using MiniShop.Models;
using System.Reflection.Emit;

namespace MiniShop.Data
{
    public class ShopDBContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(e =>
            {
                e.Property(t => t.Title).HasMaxLength(100).IsRequired();
                e.Property(t => t.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.Property(o => o.CustomerName).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<OrderItem>(e =>
            {
                e.Property(oi => oi.Price).HasPrecision(18, 2);

                e.HasOne(oi => oi.Ticket)
                 .WithMany(t => t.OrderItems)
                 .HasForeignKey(oi => oi.TicketId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(oi => oi.Order)
                 .WithMany(o => o.Items)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ticket>().HasData(
                new Ticket { Id = 1, Title = "VIP Ticket", Price = 25m, Stock = 1 },
                new Ticket { Id = 2, Title = "Standard Ticket", Price = 60m, Stock = 10 },
                new Ticket { Id = 3, Title = "Eco Ticket", Price = 120m, Stock = 5 });
        }
    }
}