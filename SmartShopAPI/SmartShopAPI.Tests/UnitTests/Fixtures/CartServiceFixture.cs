using AutoMapper;

using Moq;

using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Services;
using SmartShopAPI.Tests.UnitTests.Helpers;

namespace SmartShopAPI.Tests.UnitTests.Fixtures
{
    public class CartServiceFixture
    {
        public Mock<ICartRepository> MockCartRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public CartService Service { get; }

        private List<CartItem> _cartEntities;
        public IReadOnlyList<CartItem> CartItems => _cartEntities;

        public CartServiceFixture()
        {
            MockCartRepository = new Mock<ICartRepository>();
            MockMapper = new Mock<IMapper>();

            _cartEntities = CartTestData.GetCartItems();

            SetupRepository();
            SetupMapper();

            Service = new CartService(
                MockCartRepository.Object,
                MockMapper.Object
            );
        }

        private void SetupRepository()
        {
            MockCartRepository
                .Setup(r => r.GetCartAsync(It.IsAny<int>()))
                .ReturnsAsync((int userId) =>
                    _cartEntities
                        .Where(x => x.UserId == userId)
                        .Select(x => new CartItemDto
                        {
                            Id = x.Id,
                            ProductId = x.ProductId,
                            Quantity = x.Quantity,
                            ProductName = x.Product?.Name,
                            ProductPrice = x.Product?.Price ?? 0,
                            ProductStockQuantity = x.Product?.StockQuantity ?? 0,
                            ProductImagePath = x.Product?.ImagePath
                        })
                        .ToList()
                );

            MockCartRepository
                .Setup(r => r.GetCartItemByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                    _cartEntities.FirstOrDefault(x => x.Id == id)
                );

            MockCartRepository
                .Setup(r => r.GetCartItemByUserAndProductAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int userId, int productId) =>
                    _cartEntities.FirstOrDefault(x =>
                        x.UserId == userId && x.ProductId == productId)
                );

            MockCartRepository
                .Setup(r => r.AddCartItemAsync(It.IsAny<CartItem>()))
                .Callback<CartItem>(item =>
                {
                    item.Id = _cartEntities.Max(x => x.Id) + 1;
                    _cartEntities.Add(item);
                })
                .Returns(Task.CompletedTask);

            MockCartRepository
                .Setup(r => r.DeleteCartItem(It.IsAny<CartItem>()))
                .Callback<CartItem>(item => _cartEntities.Remove(item));

            MockCartRepository
                .Setup(r => r.ClearCartAsync(It.IsAny<int>()))
                .Callback<int>(userId =>
                {
                    _cartEntities.RemoveAll(x => x.UserId == userId);
                })
                .Returns(Task.CompletedTask);

            MockCartRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
        }


        private void SetupMapper()
        {
            MockMapper
                .Setup(m => m.Map<CartItem>(It.IsAny<CreateCartItemDto>()))
                .Returns((CreateCartItemDto dto) =>
                    new CartItem
                    {
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity
                    });
        }


        public void ResetCart()
        {
            _cartEntities = CartTestData.GetCartItems();
        }
    }
}
