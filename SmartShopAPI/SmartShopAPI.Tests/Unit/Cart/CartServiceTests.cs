using FluentAssertions;
using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Exceptions;

namespace SmartShopAPI.Tests.Unit.Cart;

public class CartServiceTests : IClassFixture<CartServiceFixture>
{
    private readonly CartServiceFixture _fixture;

    public CartServiceTests(CartServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetCart_WhenCalled_ReturnsUserCartItems()
    {
        // Arrange
        _fixture.Reset();

        // Act
        var result = await _fixture.Service.GetCart(1);

        // Assert
        result.Should().HaveCount(2);
        result.First().ProductId.Should().Be(10);
    }

    [Fact]
    public async Task GetCartItemById_WhenItemExists_ReturnsItem()
    {
        // Arrange
        _fixture.Reset();

        // Act
        var item = await _fixture.Service.GetCartItemById(1);

        // Assert
        item.Should().NotBeNull();
        item.ProductId.Should().Be(10);
    }

    [Fact]
    public async Task GetCartItemById_WhenItemNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _fixture.Reset();

        // Act & Assert
        await _fixture.Service.Invoking(s => s.GetCartItemById(999))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddCartItem_WhenItemAlreadyExists_ShouldIncreaseQuantity()
    {
        // Arrange
        _fixture.Reset();
        var dto = new CreateCartItemDto { ProductId = 10, Quantity = 3 };

        // Act
        var resultId = await _fixture.Service.AddCartItem(dto, 1);

        // Assert
        resultId.Should().Be(1);
        _fixture.CartItems.First(c => c.Id == 1).Quantity.Should().Be(5);
    }

    [Fact]
    public async Task AddCartItem_WhenItemDoesNotExist_ShouldAddNewItem()
    {
        // Arrange
        _fixture.Reset();
        var dto = new CreateCartItemDto { ProductId = 99, Quantity = 2 };

        // Act
        await _fixture.Service.AddCartItem(dto, 1);

        // Assert
        _fixture.CartItems.Should().ContainSingle(c => c.ProductId == 99);
    }

    [Fact]
    public async Task UpdateCartItem_WhenItemExists_ShouldChangeQuantity()
    {
        // Arrange
        _fixture.Reset();
        var dto = new UpdateCartItemDto { Quantity = 10 };

        // Act
        await _fixture.Service.UpdateCartItem(1, dto);

        // Assert
        _fixture.CartItems.First(c => c.Id == 1).Quantity.Should().Be(10);
    }

    [Fact]
    public async Task UpdateCartItem_WhenItemNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _fixture.Reset();
        var dto = new UpdateCartItemDto { Quantity = 5 };

        // Act & Assert
        await _fixture.Service.Invoking(s => s.UpdateCartItem(999, dto))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteCartItem_WhenItemExists_ShouldRemoveItem()
    {
        // Arrange
        _fixture.Reset();

        // Act
        await _fixture.Service.DeleteCartItem(1);

        // Assert
        _fixture.CartItems.Should().NotContain(c => c.Id == 1);
    }

    [Fact]
    public async Task DeleteCartItem_WhenItemNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _fixture.Reset();

        // Act & Assert
        await _fixture.Service.Invoking(s => s.DeleteCartItem(999))
            .Should().ThrowAsync<NotFoundException>();
    }
}
