using AutoMapper;

using Moq;

using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Services;

namespace SmartShopAPI.Tests.UnitTests.Fixtures
{
    public class CartServiceFixture
    {
        public Mock<ICartRepository> MockCartRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public CartService Service { get; }

        public List<CartItem> CartItems { get; }

        public CartServiceFixture()
        {
            MockCartRepository = new Mock<ICartRepository>();
            MockMapper = new Mock<IMapper>();

            CartItems = new List<CartItem>
            {
                new() { Id = 1, UserId = 1, ProductId = 10, Quantity = 2 },
                new() { Id = 2, UserId = 1, ProductId = 11, Quantity = 1 },
                new() { Id = 3, UserId = 2, ProductId = 12, Quantity = 5 },
            };

            SetupRepository();
            SetupMapper();

            Service = new CartService(MockCartRepository.Object, MockMapper.Object);
        }

        private void SetupRepository()
        {
            MockCartRepository.Setup(r => r.GetCartAsync(It.Is<int>(u => true)))
                .ReturnsAsync((int userId) =>
                {
                    var result = CartItems.Where(c => c.UserId == userId).ToList();
                    return result;
                });

            MockCartRepository.Setup(r => r.GetCartItemByIdAsync(It.Is<int>(id => true)))
                .ReturnsAsync((int id) =>
                {
                    var found = CartItems.FirstOrDefault(c => c.Id == id);
                    return found;
                });

            MockCartRepository.Setup(r =>
                    r.GetCartItemByUserAndProductAsync(It.Is<int>(u => true), It.Is<int>(p => true)))
                .ReturnsAsync((int userId, int productId) =>
                {
                    var found = CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
                    return found;
                });

            MockCartRepository.Setup(r => r.AddCartItemAsync(It.IsAny<CartItem>()))
                .Callback((CartItem item) =>
                {
                    item.Id = CartItems.Any() ? CartItems.Max(c => c.Id) + 1 : 1;
                    if (item.UserId == 0)
                        item.UserId = 1;

                    CartItems.Add(item);
                })
                .Returns(Task.CompletedTask);

            MockCartRepository.Setup(r => r.DeleteCartItem(It.IsAny<CartItem>()))
                .Callback((CartItem item) =>
                {
                    CartItems.Remove(item);
                });

            MockCartRepository.Setup(r => r.ClearCartAsync(It.Is<int>(u => true)))
                .Callback((int userId) =>
                {
                    var count = CartItems.RemoveAll(c => c.UserId == userId);
                })
                .Returns(Task.CompletedTask);

            MockCartRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
        }

        private void SetupMapper()
        {
            MockMapper.Setup(m => m.Map<CartItem>(It.IsAny<object>()))
                .Returns((object dto) =>
                {
                    if (dto is Models.Dtos.CartItem.CreateCartItemDto createDto)
                    {
                        return new CartItem
                        {
                            ProductId = createDto.ProductId,
                            Quantity = createDto.Quantity
                        };
                    }

                    return new CartItem();
                });
        }

        public void ResetCart()
        {
            CartItems.Clear();
            CartItems.AddRange(new List<CartItem>
            {
                new() { Id = 1, UserId = 1, ProductId = 10, Quantity = 2 },
                new() { Id = 2, UserId = 1, ProductId = 11, Quantity = 1 },
                new() { Id = 3, UserId = 2, ProductId = 12, Quantity = 5 },
            });
        }
    }
}
