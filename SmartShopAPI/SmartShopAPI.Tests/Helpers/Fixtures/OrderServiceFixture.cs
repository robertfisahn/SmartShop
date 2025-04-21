using AutoMapper;

using Moq;

using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Services;

namespace SmartShopAPI.Tests.Helpers.Fixtures
{
    public class OrderServiceFixture
    {
        public Mock<IOrderRepository> MockOrderRepository { get; }
        public Mock<IMapper> MockMapper { get; }
        public Mock<IUnitOfWork> MockUnitOfWork { get; }
        public Mock<ICartService> MockCartService { get; }
        public Mock<IProductService> MockProductService { get; }
        public Mock<IAccountService> MockAccountService { get; }
        public Mock<IOrderItemRepository> MockOrderItemRepository { get; }
        public OrderService Service { get; }

        private readonly List<Order> _orders;
        private readonly List<OrderItem> _orderItems;
        private readonly List<CartItem> _cartItems;

        public List<Order> Orders => _orders;
        public List<OrderItem> OrderItems => _orderItems;
        public List<CartItem> CartItems => _cartItems;

        public OrderServiceFixture()
        {
            MockOrderRepository = new Mock<IOrderRepository>();
            MockMapper = new Mock<IMapper>();
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockCartService = new Mock<ICartService>();
            MockProductService = new Mock<IProductService>();
            MockAccountService = new Mock<IAccountService>();
            MockOrderItemRepository = new Mock<IOrderItemRepository>();

            _orderItems = BuildOrderItems();
            _orders = BuildOrders();
            _cartItems = BuildCartItems();

            SetupRepositories();
            SetupMappers();
            SetupUnitOfWork();

            Service = new OrderService(
                MockOrderRepository.Object,
                MockMapper.Object,
                MockCartService.Object,
                MockProductService.Object,
                MockAccountService.Object,
                MockOrderItemRepository.Object,
                MockUnitOfWork.Object
            );
        }
        private List<Order> BuildOrders()
        {
            var orders = new List<Order>
            {
                new()
                {
                    Id = 1,
                    TotalPrice = 200.00M,
                    UserId = 1,
                    AddressId = 101,
                    Address = new Address
                    {
                        Street = "Order1Street",
                        City = "Order1City",
                        PostalCode = "Order1PC"
                    }
                },
                new()
                {
                    Id = 2,
                    TotalPrice = 200.00M,
                    UserId = 2,
                    AddressId = 102,
                    Address = new Address
                    {
                        Street = "Order2Street",
                        City = "Order2City",
                        PostalCode = "Order2PC"
                    }
                },
                new()
                {
                    Id = 3,
                    TotalPrice = 900.00M,
                    UserId = 2,
                    AddressId = 103,
                    Address = new Address
                    {
                        Street = "Order3Street",
                        City = "Order3City",
                        PostalCode = "Order3PC"
                    }
                }
            };

            foreach (var order in orders)
            {
                order.OrderItems = _orderItems.Where(x => x.OrderId == order.Id).ToList();
            }

            return orders;
        }

        private List<OrderItem> BuildOrderItems()
        {
            return
                [
                    new() { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, Product = new Product { Name = "TestProduct1", Price = 100M } },
                    new() { Id = 2, OrderId = 2, ProductId = 2, Quantity = 1, Product = new Product { Name = "TestProduct2", Price = 200M } },
                    new() { Id = 3, OrderId = 3, ProductId = 3, Quantity = 3, Product = new Product { Name = "TestProduct3", Price = 300M } }
                ];
        }

        private List<CartItem> BuildCartItems()
        {
            return
            [
                new()
                {
                    ProductId = 1,
                    Quantity = 2,
                    Product = new Product { Id = 1, Name = "Product1", Price = 100M }
                },
                new()
                {
                    ProductId = 2,
                    Quantity = 1,
                    Product = new Product { Id = 2, Name = "Product2", Price = 150M }
                },
                new()
                {
                    ProductId = 3,
                    Quantity = 3,
                    Product = new Product { Id = 3, Name = "Product3", Price = 50M }
                }
            ];
        }

        public void ResetData()
        {
            _orderItems.Clear();
            _orderItems.AddRange(BuildOrderItems());

            _orders.Clear();
            _orders.AddRange(BuildOrders());

            _cartItems.Clear();
            _cartItems.AddRange(BuildCartItems());
            MockUnitOfWork.Invocations.Clear();
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
                .ReturnsAsync(_cartItems);

            MockAccountService.Setup(a => a.GetAddressId(It.IsAny<int>()))
                .ReturnsAsync(1);

            MockProductService.Setup(p => p.UpdateStock(It.IsAny<List<OrderItem>>()))
                .Returns(Task.CompletedTask);
        }

        private void SetupMappers()
        {
            MockMapper.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
                .Returns((IEnumerable<Order> orders) => orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    Street = o.Address.Street,
                    City = o.Address.City,
                    PostalCode = o.Address.PostalCode,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Product.Price
                    }).ToList()
                }).ToList());

            MockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
                .Returns((Order o) => new OrderDto
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    Street = o.Address.Street,
                    City = o.Address.City,
                    PostalCode = o.Address.PostalCode,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Product.Price
                    }).ToList()
                });
            MockMapper.Setup(m => m.Map<List<OrderItem>>(It.IsAny<IEnumerable<CartItem>>()))
                .Returns((IEnumerable<CartItem> cartItems) => cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Product = c.Product
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
}
