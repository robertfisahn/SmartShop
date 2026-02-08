namespace SmartShopAPI.Models.Dtos.User
{
    public class RegisterUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string PostalCode { get; set; }

    }
}
