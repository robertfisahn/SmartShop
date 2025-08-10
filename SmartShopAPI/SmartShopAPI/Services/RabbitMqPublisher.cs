using System.Text;
using System.Text.Json;

using RabbitMQ.Client;

using SmartShopAPI.Helpers;
using SmartShopAPI.Interfaces.Events;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class RabbitMqPublisher(RabbitMqConnectionHelper _rabbitConnectionHelper) : IEventPublisher
    {
        private readonly string _queueName = "order_placed";

        public async Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent)
        {
            var connection = await _rabbitConnectionHelper.CreateConnectionWithRetryAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false);

            var message = JsonSerializer.Serialize(orderEvent);
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                body: body);
        }
    }
}
