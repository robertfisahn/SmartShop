using SmartShopAPI.Entities;
using SmartShopAPI.Models;

namespace SmartShopAPI.Tests.UnitTests.Helpers;

public static class OrderTestData
{
    public static List<OrderItem> DefaultOrderItems() =>
    [
        new()
        {
            Id = 1,
            OrderId = 1,
            ProductId = 1,
            Quantity = 2,
            Product = new Product { Name = "TestProduct1", Price = 100M }
        },
        new()
        {
            Id = 2,
            OrderId = 2,
            ProductId = 2,
            Quantity = 1,
            Product = new Product { Name = "TestProduct2", Price = 200M }
        },
        new()
        {
            Id = 3,
            OrderId = 3,
            ProductId = 3,
            Quantity = 3,
            Product = new Product { Name = "TestProduct3", Price = 300M }
        }
    ];

    public static List<Order> DefaultOrders(List<OrderItem> items) =>
    [
        new()
        {
            Id = 1,
            UserId = 1,
            TotalPrice = 200m,
            AddressId = 101,
            Address = new Address
            {
                Street = "Order1Street",
                City = "Order1City",
                PostalCode = "Order1PC"
            },
            OrderItems = items.Where(i => i.OrderId == 1).ToList()
        },
        new()
        {
            Id = 2,
            UserId = 2,
            TotalPrice = 200m,
            AddressId = 102,
            Address = new Address
            {
                Street = "Order2Street",
                City = "Order2City",
                PostalCode = "Order2PC"
            },
            OrderItems = items.Where(i => i.OrderId == 2).ToList()
        },
        new()
        {
            Id = 3,
            UserId = 2,
            TotalPrice = 900m,
            AddressId = 103,
            Address = new Address
            {
                Street = "Order3Street",
                City = "Order3City",
                PostalCode = "Order3PC"
            },
            OrderItems = items.Where(i => i.OrderId == 3).ToList()
        }
    ];
}
