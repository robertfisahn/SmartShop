namespace SmartShopAPI.Models.Events;

public class OrderPlacedEvent
{
    public int OrderId { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public List<string> ProductNames { get; set; } = [];
}
