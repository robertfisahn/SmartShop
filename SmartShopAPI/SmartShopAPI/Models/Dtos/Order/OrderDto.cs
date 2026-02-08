namespace SmartShopAPI.Models.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public required string City { get; set; }
        public required string Street { get; set; }
        public required string PostalCode { get; set; }

        public required List<OrderItemDto> OrderItems { get; set; }
    }

}
