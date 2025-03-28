using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetCartAsync(int userId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemByUserAndProductAsync(int userId, int productId);
        Task AddCartItemAsync(CartItem cartItem);
        void DeleteCartItem(CartItem cartItem);
        Task ClearCartAsync(int userId);
        Task SaveChangesAsync();
    }
}
