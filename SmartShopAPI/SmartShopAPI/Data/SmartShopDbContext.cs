using MassTransit;

using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Entities;
using SmartShopAPI.Models;

namespace SmartShopAPI.Data
{
    public class SmartShopDbContext : DbContext
    {
        public SmartShopDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .Property(i => i.Quantity)
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .Property(c => c.Quantity)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .IsRequired();

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);
            });

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(8, 2);
            });

            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
