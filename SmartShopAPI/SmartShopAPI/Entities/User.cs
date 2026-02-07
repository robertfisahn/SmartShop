namespace SmartShopAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
    }
}
