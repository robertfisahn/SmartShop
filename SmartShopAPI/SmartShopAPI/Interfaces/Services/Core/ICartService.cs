using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetCart(int userId);
        Task<int> AddCartItem(CreateCartItemDto dto, int userId);
        Task<CartItem> GetCartItemById(int cartItemId);
        Task DeleteCartItem(int cartItemId);
        Task UpdateCartItem(int cartItemId, UpdateCartItemDto dto);
        Task ClearCart(int userId);
    }
}
