using AutoMapper;

using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Services
{
    public class CartService(ICartRepository cartRepository, IMapper mapper) : ICartService
    {

        public async Task<IEnumerable<CartItem>> GetCart(int userId) => await cartRepository.GetCartAsync(userId);

        public async Task<CartItem> GetCartItemById(int cartItemId) =>
            await cartRepository.GetCartItemByIdAsync(cartItemId) ?? throw new NotFoundException("Cart item not found");

        public async Task<int> AddCartItem(CreateCartItemDto dto, int userId)
        {
            var existingCartItem = await cartRepository.GetCartItemByUserAndProductAsync(userId, dto.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += dto.Quantity;
            }
            else
            {
                var newCartItem = mapper.Map<CartItem>(dto);
                newCartItem.UserId = userId;
                await cartRepository.AddCartItemAsync(newCartItem);
                existingCartItem = newCartItem;
            }
            await cartRepository.SaveChangesAsync();
            return existingCartItem.Id;
        }

        public async Task DeleteCartItem(int cartItemId)
        {
            var cartItem = await cartRepository.GetCartItemByIdAsync(cartItemId) ?? throw new NotFoundException("Cart item not found");

            cartRepository.DeleteCartItem(cartItem);
            await cartRepository.SaveChangesAsync();
        }

        public async Task UpdateCartItem(int cartItemId, UpdateCartItemDto dto)
        {
            var cartItem = await cartRepository.GetCartItemByIdAsync(cartItemId) ?? throw new NotFoundException("Cart item not found");

            cartItem.Quantity = dto.Quantity;
            await cartRepository.SaveChangesAsync();
        }

        public async Task ClearCart(int userId)
        {
            await cartRepository.ClearCartAsync(userId);
            await cartRepository.SaveChangesAsync();
        }
    }
}
