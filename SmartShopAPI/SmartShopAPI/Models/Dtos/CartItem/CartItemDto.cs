namespace SmartShopAPI.Models.Dtos.CartItem
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = default!;
        public decimal ProductPrice { get; set; }
        public int ProductStockQuantity { get; set; }
        public string ProductImagePath { get; set; } = default!;
    }
}
