using SmartShopAPI.Interfaces.Services.Infrastructure;

namespace SmartShopAPI.Entities
{
    public class CartItem : IUserVerification
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
