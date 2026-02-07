namespace SmartShopAPI.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string PostalCode { get; set; }
        public bool IsDefault { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
