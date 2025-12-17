using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Repositories
{
    public class CartRepository(SmartShopDbContext context) : ICartRepository
    {
        public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId) =>
            await context.CartItems
                .Where(x => x.UserId == userId)
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    ProductPrice = x.Product.Price,
                    ProductStockQuantity = x.Product.StockQuantity,
                    ProductImagePath = x.Product.ImagePath
                })
                .ToListAsync();

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
