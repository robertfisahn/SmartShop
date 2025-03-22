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

        public IEnumerable<CartItem> GetUserCart(int userId) => cartRepository.GetUserCart(userId);

        public CartItem GetCartItem(int cartItemId) =>
            cartRepository.GetCartItem(cartItemId) ?? throw new NotFoundException("Cart item not found");


        public int AddCartItem(CreateCartItemDto dto, int userId)
        {
            var existingCartItem = cartRepository.GetCartItemByUserAndProduct(userId, dto.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += dto.Quantity;
                cartRepository.UpdateCartItem(existingCartItem);
            }
            else
            {
                var newCartItem = mapper.Map<CartItem>(dto);
                newCartItem.UserId = userId;
                cartRepository.AddCartItem(newCartItem);
                existingCartItem = newCartItem;
            }
            cartRepository.SaveChanges();
            return existingCartItem.Id;
        }

        public void DeleteCartItem(int cartItemId)
        {
            var cartItem = cartRepository.GetCartItem(cartItemId) ?? throw new NotFoundException("Cart item not found");

            cartRepository.DeleteCartItem(cartItem);
            cartRepository.SaveChanges();
        }

        public void UpdateCartItem(int cartItemId, UpdateCartItemDto dto)
        {
            var cartItem = cartRepository.GetCartItem(cartItemId) ?? throw new NotFoundException("Cart item not found");

            cartItem.Quantity = dto.Quantity;
            cartRepository.SaveChanges();
        }

        public void ClearCart(int userId)
        {
            cartRepository.ClearCart(userId);
            cartRepository.SaveChanges();
        }
    }
}
