namespace SmartShopAPI.Models.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public required string CreatedDate { get; set; }
        public required string Address { get; set; }
        public required List<OrderItemDto> OrderItems { get; set; }
    }
}
