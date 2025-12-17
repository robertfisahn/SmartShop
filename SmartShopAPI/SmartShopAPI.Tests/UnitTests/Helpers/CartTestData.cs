using SmartShopAPI.Entities;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Tests.UnitTests.Helpers;

public static class CartTestData
{
    public static List<CartItemDto> GetCartItemDtos() =>
    [
        new CartItemDto
        {
            Id = 1,
            Quantity = 2,
            ProductId = 10,
            ProductName = "Test product 1",
            ProductPrice = 100m,
            ProductStockQuantity = 10,
            ProductImagePath = "img1.png"
        },
        new CartItemDto
        {
            Id = 2,
            Quantity = 1,
            ProductId = 11,
            ProductName = "Test product 2",
            ProductPrice = 50m,
            ProductStockQuantity = 5,
            ProductImagePath = "img2.png"
        }
    ];
    public static List<CartItem> GetCartItems() =>
    [
        new CartItem
        {
            Id = 1,
            UserId = 1,
            ProductId = 10,
            Quantity = 2,
            Product = new Product
            {
                Id = 10,
                Name = "Test product 1",
                Price = 100m,
                StockQuantity = 10,
                ImagePath = "img1.png"
            }
        },
        new CartItem
        {
            Id = 2,
            UserId = 1,
            ProductId = 11,
            Quantity = 1,
            Product = new Product
            {
                Id = 11,
                Name = "Test product 2",
                Price = 50m,
                StockQuantity = 5,
                ImagePath = "img2.png"
            }
        }
    ];
}
