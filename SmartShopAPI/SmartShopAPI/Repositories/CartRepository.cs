using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class CartRepository(SmartShopDbContext context) : ICartRepository
    {
        public IEnumerable<CartItem> GetUserCart(int userId) =>
            context.CartItems.Include(p => p.Product).Where(x => x.UserId == userId).ToList();

        public CartItem? GetCartItem(int cartItemId) =>
            context.CartItems.Include(p => p.Product).FirstOrDefault(x => x.Id == cartItemId);

        public CartItem? GetCartItemByUserAndProduct(int userId, int productId)
        {
            return context.CartItems
                .FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public void AddCartItem(CartItem cartItem)
        {
            context.CartItems.Add(cartItem);
        }

        public void DeleteCartItem(CartItem cartItem)
        {
            context.CartItems.Remove(cartItem);
        }

        public void UpdateCartItem(CartItem cartItem)
        {
            context.CartItems.Update(cartItem);
        }

        public void ClearCart(int userId)
        {
            var cartItems = context.CartItems.Where(c => c.UserId == userId);
            context.RemoveRange(cartItems);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
