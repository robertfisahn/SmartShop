namespace SmartShopAPI.Models.Dtos.User
{
    public class ShippingAddressDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
    }
}
