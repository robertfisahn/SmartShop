using SmartShopAPI.Interfaces.Services.Infrastructure;
using SmartShopAPI.Models.Enums;

namespace SmartShopAPI.Entities
{
    public class Order : IUserVerification
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string? PaymentMethod { get; set; }
        public string? PaymentReference { get; set; }

        public required string ShippingCity { get; set; }
        public required string ShippingStreet { get; set; }
        public required string ShippingPostalCode { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
