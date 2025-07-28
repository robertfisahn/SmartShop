using System.Text;
using System.Text.Json;

using RabbitMQ.Client;

using SmartShopAPI.Interfaces.Events;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class RabbitMqPublisher(RabbitMqSettings _settings) : IEventPublisher
    {
        private readonly string _queueName = "order_placed";

        public async Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host ?? "localhost",
                Port = _settings.Port > 0 ? _settings.Port : 5672,
                UserName = _settings.UserName ?? "guest",
                Password = _settings.Password ?? "guest"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

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
