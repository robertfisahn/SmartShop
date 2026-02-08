namespace SmartShopAPI.Models.Dtos.User
{
    public class ShippingAddressDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
    }
}
