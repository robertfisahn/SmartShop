using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface ICartRepository
    {
        IEnumerable<CartItem> GetUserCart(int userId);
        CartItem? GetCartItem(int cartItemId);
        CartItem? GetCartItemByUserAndProduct(int userId, int productId);
        void AddCartItem(CartItem cartItem);
        void DeleteCartItem(CartItem cartItem);
        void UpdateCartItem(CartItem cartItem);
        void ClearCart(int userId);
        void SaveChanges();
    }
}
