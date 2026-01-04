using SmartShopAPI.Interfaces.Services.Infrastructure;
using SmartShopAPI.Models;

namespace SmartShopAPI.Entities
{
    public class CartItem : IUserVerification
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
