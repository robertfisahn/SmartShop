using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class CartRepository(SmartShopDbContext context) : ICartRepository
    {
        public async Task<IEnumerable<CartItem>> GetCartAsync(int userId) =>
            await context.CartItems.Include(p => p.Product).Where(x => x.UserId == userId).ToListAsync();

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId) =>
            await context.CartItems.Include(p => p.Product).FirstOrDefaultAsync(x => x.Id == cartItemId);

        public async Task<CartItem?> GetCartItemByUserAndProductAsync(int userId, int productId) =>
            await context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        public async Task AddCartItemAsync(CartItem cartItem) =>
            await context.CartItems.AddAsync(cartItem);

        public void DeleteCartItem(CartItem cartItem)
        {
            context.CartItems.Remove(cartItem);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = await context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            context.RemoveRange(cartItems);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
