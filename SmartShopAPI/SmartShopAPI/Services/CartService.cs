using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Services
{
    public class CartService(SmartShopDbContext context, IMapper mapper) : ICartService
    {

        public IEnumerable<CartItem> GetUserCart(int userId)
        {
            var cartItems = context.CartItems
                .Include(p => p.Product)
                .Where(x => x.UserId == userId)
                .ToList();
            return cartItems;
        }

        public CartItem GetCartItem(int cartItemId) {
            var cartItem = context.CartItems
                .Include(p => p.Product)
                .FirstOrDefault(x => x.Id == cartItemId) ?? throw new NotFoundException("Cart item not found");
            return cartItem;
        }

        public int AddCartItem(CreateCartItemDto dto, int userId)
        {
            var existingCartItem = context.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId && ci.UserId == userId);
            CartItem cartItem;
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += dto.Quantity;
                cartItem = existingCartItem;
            }
            else
            {
                cartItem = mapper.Map<CartItem>(dto);
                cartItem.UserId = userId;
                context.CartItems.Add(cartItem);
            }
            context.SaveChanges();
            return cartItem.Id;
        }

        public void DeleteCartItem(int cartItemId)
        {
            var cartItem = context.CartItems
                .FirstOrDefault(x => x.Id == cartItemId) ?? throw new NotFoundException("Cart item not found"); 

            context.CartItems.Remove(cartItem);
            context.SaveChanges();
        }

        public void UpdateCartItem(int cartItemId, UpdateCartItemDto dto)
        {
            var cartItem = context.CartItems
                .FirstOrDefault(x => x.Id == cartItemId) ?? throw new NotFoundException("Cart item not found");         

            cartItem.Quantity = dto.Quantity;
            context.SaveChanges();
        }

        public void ClearCartItems(int userId)
        {
            var cartItems = context.CartItems.Where(c => c.UserId == userId);
            context.RemoveRange(cartItems);
            context.SaveChanges();
        }
    }
}
