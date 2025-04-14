using SmartShopAPI.Interfaces;

namespace SmartShopAPI.Entities
{
    public class Order : IUserVerification
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int AddressId { get; set; }
        public virtual Address Address { get; set; }
        public virtual List<OrderItem> OrderItems { get; set; }
    }
}
