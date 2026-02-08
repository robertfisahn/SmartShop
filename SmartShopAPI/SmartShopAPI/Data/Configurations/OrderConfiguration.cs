using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartShopAPI.Entities;

namespace SmartShopAPI.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalPrice)
                .IsRequired()
                .HasPrecision(8, 2);

            builder.Property(o => o.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(o => o.PaymentReference)
                .HasMaxLength(150);

            builder.Property(o => o.ShippingCity)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.ShippingStreet)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.ShippingPostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
