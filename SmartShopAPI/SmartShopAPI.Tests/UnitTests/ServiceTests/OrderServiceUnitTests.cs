using FluentAssertions;

using Moq;

using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Tests.Helpers.Fixtures;

namespace SmartShopAPI.Tests.UnitTests.ServiceTests;

public class OrderServiceTests(OrderServiceFixture fixture) : IClassFixture<OrderServiceFixture>
{
    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenExistsAndBelongsToUser()
    {
        var result = await fixture.Service.GetById(1, 1);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ShouldThrowNotFound_WhenOrderDoesNotExist()
    {
        await FluentActions.Invoking(() => fixture.Service.GetById(999, 1))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Order not found");
    }

    [Fact]
    public async Task GetById_ShouldThrowNotFound_WhenOrderBelongsToAnotherUser()
    {
        await FluentActions.Invoking(() => fixture.Service.GetById(2, 1))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Order not found");
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnOrders_ForValidUser()
    {
        var result = await fixture.Service.GetUserOrders(2);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnEmpty_WhenUserHasNoOrders()
    {
        var result = await fixture.Service.GetUserOrders(99);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task PlaceOrder_ShouldCreateNewOrder_AndOrderItems()
    {
        fixture.ResetData();
        var resultId = await fixture.Service.PlaceOrder(1);

        fixture.Orders.Should().Contain(o => o.Id == resultId);
        fixture.OrderItems.Should().Contain(i => i.OrderId == resultId);
        resultId.Should().BeGreaterThan(3);
    }

    [Fact]
    public async Task PlaceOrder_ShouldSetCorrectTotalPrice()
    {
        fixture.ResetData();
        var resultId = await fixture.Service.PlaceOrder(1);

        var createdOrder = fixture.Orders.First(o => o.Id == resultId);
        createdOrder.TotalPrice.Should().Be(500);
    }

    [Fact]
    public async Task PlaceOrder_ShouldCallSaveChangesAsync()
    {
        fixture.ResetData();
        await fixture.Service.PlaceOrder(1);

        fixture.MockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task PlaceOrder_ShouldCommitTransaction()
    {
        fixture.ResetData();
        await fixture.Service.PlaceOrder(1);
        fixture.MockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
        fixture.MockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        fixture.MockUnitOfWork.Verify(uow => uow.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task PlaceOrder_ShouldRollback_WhenExceptionOccurs()
    {
        try
        {
            fixture.MockProductService
                .Setup(p => p.UpdateStock(It.IsAny<List<OrderItem>>()))
                .ThrowsAsync(new Exception("Simulated failure"));

            await FluentActions
                .Invoking(() => fixture.Service.PlaceOrder(1))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Simulated failure");

            fixture.MockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }
        finally
        {
            fixture.MockProductService
                .Setup(p => p.UpdateStock(It.IsAny<List<OrderItem>>()))
                .Returns(Task.CompletedTask);
        }
    }

    [Fact]
    public async Task PlaceOrder_ShouldPublishOrderPlacedEvent_WithCorrectData()
    {
        fixture.ResetData();
        int userId = 1;
        var resultId = await fixture.Service.PlaceOrder(userId);
        var createdOrder = fixture.Orders.First(o => o.Id == resultId);

        fixture.MockEventPublisher.Verify(
            m => m.PublishOrderPlacedAsync(It.Is<SmartShopAPI.Models.Events.OrderPlacedEvent>(
                evt =>
                    evt.OrderId == resultId &&
                    !string.IsNullOrEmpty(evt.Email) &&
                    evt.TotalPrice == createdOrder.TotalPrice
            )),
            Times.Once
        );
    }
}
