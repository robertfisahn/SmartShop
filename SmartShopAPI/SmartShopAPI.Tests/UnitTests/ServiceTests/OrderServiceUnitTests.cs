using FluentAssertions;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Tests.Helpers.Fixtures;

namespace SmartShopAPI.Tests.UnitTests.ServiceTests;

public class OrderServiceTests : IClassFixture<OrderServiceFixture>
{
    private readonly OrderServiceFixture _fixture;

    public OrderServiceTests(OrderServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenExistsAndBelongsToUser()
    {
        var result = await _fixture.Service.GetById(1, 1);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ShouldThrowNotFound_WhenOrderDoesNotExist()
    {
        await FluentActions.Invoking(() => _fixture.Service.GetById(999, 1))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Order not found");
    }

    [Fact]
    public async Task GetById_ShouldThrowNotFound_WhenOrderBelongsToAnotherUser()
    {
        await FluentActions.Invoking(() => _fixture.Service.GetById(2, 1))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Order not found");
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnOrders_ForValidUser()
    {
        var result = await _fixture.Service.GetUserOrders(2);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnEmpty_WhenUserHasNoOrders()
    {
        var result = await _fixture.Service.GetUserOrders(99);

        result.Should().NotBeNull();
        result.Should().BeEmpty(); 
    }

    [Fact]
    public async Task PlaceOrder_ShouldCreateNewOrder_AndOrderItems()
    {
        var resultId = await _fixture.Service.PlaceOrder(1);

        _fixture.Orders.Should().Contain(o => o.Id == resultId);
        _fixture.OrderItems.Should().Contain(i => i.OrderId == resultId);
        resultId.Should().BeGreaterThan(3);
    }

    [Fact]
    public async Task PlaceOrder_ShouldSetCorrectTotalPrice()
    {
        var resultId = await _fixture.Service.PlaceOrder(1);

        var createdOrder = _fixture.Orders.First(o => o.Id == resultId);
        createdOrder.TotalPrice.Should().Be(500);
    }
}
