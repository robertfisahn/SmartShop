using AutoMapper;
using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.Order;

namespace SmartShopAPI.Services
{
    public class OrderService(IOrderRepository orderRepository, IMapper mapper, ICartService cartService, 
        IProductService productService, IAccountService accountService, IOrderItemRepository orderItemRepository,
        IUnitOfWork unitOfWork) : IOrderService
    {
        public async Task<OrderDto> GetById(int orderId, int userId)
        {
            var order = await orderRepository.GetAsync(orderId, userId) ?? throw new NotFoundException("Order not found");        
            return mapper.Map<OrderDto>(order);
        }

        public async Task<int> PlaceOrder(int userId)
        {
            var cartItems = await cartService.GetCart(userId);

            await unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await CreateOrder(cartItems, userId);
                await unitOfWork.SaveChangesAsync(); // < save for order id
                var orderItems = await CreateOrderItems(cartItems, order.Id);

                await productService.UpdateStock(orderItems);
                await cartService.ClearCart(userId);
                await unitOfWork.SaveChangesAsync();

                await unitOfWork.CommitAsync();
                return order.Id;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
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
            return order;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrders(int userId)
        {
            var orders = await orderRepository.GetUserOrdersAsync(userId);
            return mapper.Map<IEnumerable<OrderDto>>(orders);
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
