using AutoMapper;

using MassTransit;

using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class OrderService(IOrderRepository orderRepository, IMapper mapper, ICartService cartService,
        IProductService productService, IAccountService accountService, IOrderItemRepository orderItemRepository,
        IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint, IPaymentService paymentService) : IOrderService
    {
        public async Task<OrderDto> GetById(int orderId, int userId)
        {
            var order = await orderRepository.GetAsync(orderId, userId) ?? throw new NotFoundException("Order not found");
            return mapper.Map<OrderDto>(order);
        }

        public async Task<PlaceOrderResponse> PlaceOrder(int userId, string provider)
        {
            var cartItems = await cartService.GetCart(userId);

            await unitOfWork.BeginTransactionAsync();

            Order order;
            try
            {
                order = await CreateOrder(cartItems, userId);
                await unitOfWork.SaveChangesAsync();

                var orderItems = await CreateOrderItems(cartItems, order.Id);
                await productService.UpdateStock(orderItems);
                await cartService.ClearCart(userId);

                var orderEvent = new OrderPlacedEvent
                {
                    OrderId = order.Id,
                    Email = await accountService.GetEmailByIdAsync(userId),
                    TotalPrice = order.TotalPrice,
                    ProductNames = cartItems.Select(x => x.ProductName).ToList()
                };
                await publishEndpoint.Publish(orderEvent);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }

            var payment = await paymentService.StartPaymentAsync(order.Id, order.TotalPrice, provider);

            return new PlaceOrderResponse
            {
                OrderId = order.Id,
                PaymentUrl = payment.PaymentUrl
            };
        }

        private async Task<Order> CreateOrder(IEnumerable<CartItemDto> cartItems, int userId)
        {
            var addressId = await accountService.GetAddressId(userId);
            Order order = new()
            {
                TotalPrice = cartItems.Sum(x => x.Quantity * x.ProductPrice),
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

        private async Task<List<OrderItem>> CreateOrderItems(IEnumerable<CartItemDto> cartItems, int orderId)
        {
            var orderItems = mapper.Map<List<OrderItem>>(cartItems);
            foreach (var item in orderItems)
            {
                item.OrderId = orderId;
            }
            await orderItemRepository.AddOrderItemsAsync(orderItems);
            return orderItems;
        }

        public async Task<CheckoutDataDto> GetCheckoutData(int userId)
        {
            var cartItems = await cartService.GetCart(userId);
            var address = await accountService.GetShippingAddress(userId);
            var providers = paymentService.GetAvailableProviders();

            return new CheckoutDataDto
            {
                CartItems = cartItems.ToList(),
                Address = address,
                Providers = providers
            };
        }

    }
}
