using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Interfaces.Services
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetUserCart(int userId);
        int AddCartItem(CreateCartItemDto dto, int userId);
        CartItem GetCartItem(int cartItemId);
        void DeleteCartItem(int cartItemId);
        void UpdateCartItem(int cartItemId, UpdateCartItemDto dto);
        void ClearCart(int userId);
    }
}