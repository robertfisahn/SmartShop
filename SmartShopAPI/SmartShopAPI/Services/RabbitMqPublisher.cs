using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using SmartShopAPI.Helpers;
using SmartShopAPI.Interfaces.Events;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class RabbitMqPublisher(
        RabbitMqConnectionHelper _rabbitConnectionHelper,
        ILogger<RabbitMqPublisher> _logger
    ) : IEventPublisher
    {
        private const string QueueName = "order_placed";

        public async Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent)
        {
            var conn = await _rabbitConnectionHelper.CreateConnectionWithRetryAsync();
            await using var ch = await conn.CreateChannelAsync(new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true));

            ch.BasicReturnAsync += (_, a) =>
            {
                _logger.LogWarning("BASIC.RETURN code={Code} rk={RK} reply='{Reply}'", a.ReplyCode, a.RoutingKey, a.ReplyText);
                return Task.CompletedTask;
            };

            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    await ch.QueueDeclarePassiveAsync(QueueName);
                    if (i > 1) _logger.LogInformation("Queue '{Queue}' became available after {Attempt} attempts.", QueueName, i);
                    break;
                }
                catch (OperationInterruptedException ex) when (ex.ShutdownReason?.ReplyCode == 404)
                {
                    if (i == 5)
                    {
                        _logger.LogError(ex, "Queue '{Queue}' not found after {Attempts} attempts.", QueueName, i);
                        throw new InvalidOperationException($"Queue '{QueueName}' not found.", ex);
                    }

                    var delay = 200 * i;
                    _logger.LogInformation("Queue '{Queue}' not found (attempt {Attempt}/5). Retrying in {Delay}ms...", QueueName, i, delay);
                    await Task.Delay(delay);
                }
            }

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderEvent));
            var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

            _logger.LogInformation("Publishing OrderPlaced event to '{Queue}'...", QueueName);
            await ch.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: true,
                basicProperties: props,
                body: body);
            _logger.LogInformation("Publish completed to '{Queue}'.", QueueName);
        }
    }
}
