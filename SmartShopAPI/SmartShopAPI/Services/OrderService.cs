using AutoMapper;
using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;

namespace SmartShopAPI.Services
{
    public class OrderService(IOrderRepository orderRepository, IMapper mapper, ICartService cartService, 
        IProductService productService, IAccountService accountService, IOrderItemRepository orderItemRepository) : IOrderService
    {
        public Order GetById(int orderId)
        {
            var order = orderRepository.GetOrder(orderId) ?? throw new NotFoundException("Order not found");        
            return order;
        }

        public int AddOrder(int userId)
        {
            var cartItems = cartService.GetUserCart(userId);
            Order order = CreateOrder(cartItems, userId);
            var orderItems = CreateOrderItems(cartItems, order.Id);
            productService.UpdateStockQuantity(orderItems);
            cartService.ClearCart(userId);
            orderRepository.SaveChanges();
            return order.Id;
        }

        public Order CreateOrder(IEnumerable<CartItem> cartItems, int userId)
        {
            var addressId = accountService.GetUserAddressId(userId);
            Order order = new()
            {
                TotalPrice = cartItems.Sum(x => x.Quantity * x.Product.Price),
                UserId = userId,
                AddressId = addressId
            };
            orderRepository.AddOrder(order);
            orderRepository.SaveChanges();
            return order;
        }

        public IEnumerable<Order> GetUserOrders(int userId)
        {
            var orders = orderRepository.GetUserOrders(userId);
            return orders;
        }

        public List<OrderItem> CreateOrderItems(IEnumerable<CartItem> cartItems, int orderId)
        {
            var orderItems = mapper.Map<List<OrderItem>>(cartItems);
            foreach (var item in orderItems)
            {
                item.OrderId = orderId;
            }
            orderItemRepository.AddOrderItems(orderItems);
            return orderItems;
        }
    }


}
