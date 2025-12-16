using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItemDto>> GetCartAsync(int userId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemByUserAndProductAsync(int userId, int productId);
        Task AddCartItemAsync(CartItem cartItem);
        void DeleteCartItem(CartItem cartItem);
        Task ClearCartAsync(int userId);
        Task SaveChangesAsync();
    }
}
