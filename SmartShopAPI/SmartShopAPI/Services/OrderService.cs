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
        public async Task<Order> GetById(int orderId)
        {
            var order = await orderRepository.GetAsync(orderId) ?? throw new NotFoundException("Order not found");        
            return order;
        }

        public async Task<int> PlaceOrder(int userId)
        {
            var cartItems = await cartService.GetCart(userId);
            Order order = await CreateOrder(cartItems, userId);
            var orderItems = await CreateOrderItems(cartItems, order.Id);
            await productService.UpdateStock(orderItems);
            await cartService.ClearCart(userId);
            await orderRepository.SaveChangesAsync();
            return order.Id;
        }

        private async Task<Order> CreateOrder(IEnumerable<CartItem> cartItems, int userId)
        {
            var addressId = await accountService.GetAddressId(userId);
            Order order = new()
            {
                TotalPrice = cartItems.Sum(x => x.Quantity * x.Product.Price),
                UserId = userId,
                AddressId = addressId
            };
            await orderRepository.AddAsync(order);
            await orderRepository.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrders(int userId)
        {
            var orders = await orderRepository.GetUserOrdersAsync(userId);
            return orders;
        }

        private async Task<List<OrderItem>> CreateOrderItems(IEnumerable<CartItem> cartItems, int orderId)
        {
            var orderItems = mapper.Map<List<OrderItem>>(cartItems);
            foreach (var item in orderItems)
            {
                item.OrderId = orderId;
            }
            await orderItemRepository.AddOrderItemsAsync(orderItems);
            return orderItems;
        }
    }


}
