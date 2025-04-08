using FluentAssertions;
using Moq;
using SmartShopAPI.Entities;
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
        _fixture.ResetData();
        var resultId = await _fixture.Service.PlaceOrder(1);

        _fixture.Orders.Should().Contain(o => o.Id == resultId);
        _fixture.OrderItems.Should().Contain(i => i.OrderId == resultId);
        resultId.Should().BeGreaterThan(3);
    }

    [Fact]
    public async Task PlaceOrder_ShouldSetCorrectTotalPrice()
    {
        _fixture.ResetData();
        var resultId = await _fixture.Service.PlaceOrder(1);

        var createdOrder = _fixture.Orders.First(o => o.Id == resultId);
        createdOrder.TotalPrice.Should().Be(500);
    }

    [Fact]
    public async Task PlaceOrder_ShouldCallSaveChangesAsync()
    {
        _fixture.ResetData();
        await _fixture.Service.PlaceOrder(1);
        
        _fixture.MockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task PlaceOrder_ShouldCommitTransaction()
    {
        _fixture.ResetData();
        await _fixture.Service.PlaceOrder(1);
        _fixture.MockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
        _fixture.MockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        _fixture.MockUnitOfWork.Verify(uow => uow.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task PlaceOrder_ShouldRollback_WhenExceptionOccurs()
    {
        try
        {
            _fixture.MockProductService
                .Setup(p => p.UpdateStock(It.IsAny<List<OrderItem>>()))
                .ThrowsAsync(new Exception("Simulated failure"));

            await FluentActions
                .Invoking(() => _fixture.Service.PlaceOrder(1))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Simulated failure");

            _fixture.MockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
        finally
        {
            _fixture.MockProductService
                .Setup(p => p.UpdateStock(It.IsAny<List<OrderItem>>()))
                .Returns(Task.CompletedTask);
        }
    }
}
