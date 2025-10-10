using FluentAssertions;

using SmartShopAPI.Exceptions;
using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Tests.Helpers.Fixtures;

namespace SmartShopAPI.Tests.UnitTests.ServiceTests
{
    public class CartServiceUnitTests : IClassFixture<CartServiceFixture>
    {
        private readonly CartServiceFixture fixture;

        public CartServiceUnitTests(CartServiceFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetCart_ReturnsUserCartItems()
        {
            fixture.ResetCart();
            var result = await fixture.Service.GetCart(1);

            result.Should().HaveCount(2);
            result.First().ProductId.Should().Be(10);
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnItem_WhenExists()
        {
            fixture.ResetCart(); 
            var item = await fixture.Service.GetCartItemById(1);

            item.Should().NotBeNull();
            item.ProductId.Should().Be(10);
        }

        [Fact]
        public async Task GetCartItemById_ShouldThrow_WhenNotFound()
        {
            fixture.ResetCart();
            await Assert.ThrowsAsync<NotFoundException>(() => fixture.Service.GetCartItemById(999));
        }

        [Fact]
        public async Task AddCartItem_ShouldIncreaseQuantity_WhenAlreadyExists()
        {
            fixture.ResetCart();
            var dto = new CreateCartItemDto { ProductId = 10, Quantity = 3 };

            var resultId = await fixture.Service.AddCartItem(dto, 1);

            resultId.Should().Be(1);
            fixture.CartItems.First(c => c.Id == 1).Quantity.Should().Be(5);
        }

        [Fact]
        public async Task AddCartItem_ShouldAddNewItem_WhenNotExists()
        {
            fixture.ResetCart();
            var dto = new CreateCartItemDto { ProductId = 99, Quantity = 2 };

            var newId = await fixture.Service.AddCartItem(dto, 1);

            fixture.CartItems.Should().ContainSingle(c => c.ProductId == 99);
            newId.Should().BeGreaterThan(3);
        }

        [Fact]
        public async Task UpdateCartItem_ShouldChangeQuantity()
        {
            fixture.ResetCart();
            var dto = new UpdateCartItemDto { Quantity = 10 };

            await fixture.Service.UpdateCartItem(1, dto);

            fixture.CartItems.First(c => c.Id == 1).Quantity.Should().Be(10);
        }

        [Fact]
        public async Task UpdateCartItem_ShouldThrow_WhenItemNotFound()
        {
            fixture.ResetCart();
            var dto = new UpdateCartItemDto { Quantity = 5 };

            await Assert.ThrowsAsync<NotFoundException>(() => fixture.Service.UpdateCartItem(999, dto));
        }

        [Fact]
        public async Task DeleteCartItem_ShouldRemove_WhenExists()
        {
            fixture.ResetCart();
            await fixture.Service.DeleteCartItem(1);

            fixture.CartItems.Should().NotContain(c => c.Id == 1);
        }

        [Fact]
        public async Task DeleteCartItem_ShouldThrow_WhenNotFound()
        {
            fixture.ResetCart();
            await Assert.ThrowsAsync<NotFoundException>(() => fixture.Service.DeleteCartItem(999));
        }

        [Fact]
        public async Task ClearCart_ShouldRemoveAllUserItems()
        {
            fixture.ResetCart();
            await fixture.Service.ClearCart(1);

            fixture.CartItems.Should().OnlyContain(c => c.UserId != 1);
        }
    }
}
