using AutoMapper;
using MassTransit;
using Moq;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Dtos.User;
using SmartShopAPI.Services.Core;
using SmartShopAPI.Tests.Unit.Cart;

namespace SmartShopAPI.Tests.Unit.Orders;

public class OrderServiceFixture
{
    public Mock<IOrderRepository> MockOrderRepository { get; }
    public Mock<IMapper> MockMapper { get; }
    public Mock<IUnitOfWork> MockUnitOfWork { get; }
    public Mock<ICartService> MockCartService { get; }
    public Mock<IProductService> MockProductService { get; }
    public Mock<IUserService> MockUserService { get; }
    public Mock<IOrderItemRepository> MockOrderItemRepository { get; }
    public Mock<IPublishEndpoint> MockPublishEndpoint { get; }
    public Mock<IPaymentService> MockPaymentService { get; }

    public OrderService Service { get; }

    private readonly List<Order> _orders;
    private readonly List<OrderItem> _orderItems;

    public List<Order> Orders => _orders;
    public List<OrderItem> OrderItems => _orderItems;

    public OrderServiceFixture()
    {
        MockOrderRepository = new Mock<IOrderRepository>();
        MockMapper = new Mock<IMapper>();
        MockUnitOfWork = new Mock<IUnitOfWork>();
        MockCartService = new Mock<ICartService>();
        MockProductService = new Mock<IProductService>();
        MockUserService = new Mock<IUserService>();
        MockOrderItemRepository = new Mock<IOrderItemRepository>();
        MockPublishEndpoint = new Mock<IPublishEndpoint>();
        MockPaymentService = new Mock<IPaymentService>();

        _orderItems = OrderTestData.DefaultOrderItems();
        _orders = OrderTestData.DefaultOrders(_orderItems);

        SetupRepositories();
        SetupMappers();
        SetupUnitOfWork();

        Service = new OrderService(
            MockOrderRepository.Object,
            MockMapper.Object,
            MockCartService.Object,
            MockProductService.Object,
            MockUserService.Object,
            MockOrderItemRepository.Object,
            MockUnitOfWork.Object,
            MockPublishEndpoint.Object,
            MockPaymentService.Object
        );
    }

    public void Reset()
    {
        _orderItems.Clear();
        _orderItems.AddRange(OrderTestData.DefaultOrderItems());

        _orders.Clear();
        _orders.AddRange(OrderTestData.DefaultOrders(_orderItems));

        MockOrderRepository.Invocations.Clear();
        MockOrderItemRepository.Invocations.Clear();
        MockCartService.Invocations.Clear();
        MockUserService.Invocations.Clear();
        MockProductService.Invocations.Clear();
        MockPaymentService.Invocations.Clear();
        MockUnitOfWork.Invocations.Clear();
        MockPublishEndpoint.Invocations.Clear();
        MockMapper.Invocations.Clear();
    }

    private void SetupRepositories()
    {
        MockOrderRepository.Setup(r => r.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((int id, int userId) =>
                _orders.FirstOrDefault(o => o.Id == id && o.UserId == userId));

        MockOrderRepository.Setup(r => r.GetUserOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync((int userId) =>
                _orders.Where(o => o.UserId == userId).ToList());

        MockOrderRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(order =>
            {
                order.Id = _orders.Max(o => o.Id) + 1;
                _orders.Add(order);
            });

        MockOrderItemRepository.Setup(r => r.AddOrderItemsAsync(It.IsAny<List<OrderItem>>()))
            .Callback<List<OrderItem>>(items => _orderItems.AddRange(items))
            .Returns(Task.CompletedTask);

        MockCartService.Setup(c => c.GetCart(It.IsAny<int>()))
            .ReturnsAsync(CartTestData.GetCartItemDtos());

        MockUserService.Setup(u => u.GetShippingAddressAsync(It.IsAny<int>()))
            .ReturnsAsync(new ShippingAddressDto
            {
                FirstName = "Test",
                LastName = "User",
                Street = "TestStreet",
                City = "TestCity",
                PostalCode = "12345"
            });

        MockProductService.Setup(p => p.UpdateStock(It.IsAny<List<OrderItem>>()))
            .Returns(Task.CompletedTask);

        MockUserService
            .Setup(u => u.GetEmailByIdAsync(It.IsAny<int>()))
            .ReturnsAsync("testuser@example.com");

        MockPaymentService
            .Setup(p => p.StartPaymentAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new PaymentInitResult
            {
                ProviderOrderId = "TEST_ORDER_ID",
                PaymentUrl = "https://fake-payment.test"
            });
    }

    private void SetupMappers()
    {
        MockMapper.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
            .Returns((IEnumerable<Order> orders) => orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                Street = o.ShippingStreet,
                City = o.ShippingCity,
                PostalCode = o.ShippingPostalCode,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Product?.Price ?? 0
                }).ToList()
            }).ToList());

        MockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
            .Returns((Order o) => new OrderDto
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                Street = o.ShippingStreet,
                City = o.ShippingCity,
                PostalCode = o.ShippingPostalCode,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Product?.Price ?? 0
                }).ToList()
            });

        MockMapper.Setup(m => m.Map<List<OrderItem>>(It.IsAny<IEnumerable<CartItemDto>>()))
            .Returns((IEnumerable<CartItemDto> cartItems) =>
                cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Product = new Product
                    {
                        Id = c.ProductId,
                        Name = c.ProductName,
                        Price = c.ProductPrice
                    }
                }).ToList());
    }

    private void SetupUnitOfWork()
    {
        MockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        MockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        MockUnitOfWork.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
        MockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
    }
}
