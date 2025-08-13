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

            await using var channel = await connection.CreateChannelAsync(
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true
                )
            );

            channel.BasicReturnAsync += (_, args) =>
            {
                var returnedMsg = Encoding.UTF8.GetString(args.Body.ToArray());
                Console.WriteLine($"[RETURN] rk={args.RoutingKey} reply={args.ReplyText} msg={returnedMsg}");
                return Task.CompletedTask;
            };

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderEvent));
            var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                mandatory: true,
                basicProperties: props,
                body: body,
                cancellationToken: cts.Token
            );
        }

    }
}
