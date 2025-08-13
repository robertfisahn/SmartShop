using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using SmartShopAPI.Helpers;
using SmartShopAPI.Interfaces.Events;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class OrderEmailConsumer(
        IEmailSender _emailSender,
        ILogger<OrderEmailConsumer> _logger,
        RabbitMqConnectionHelper _rabbitConnectionHelper
    ) : BackgroundService
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private const string QueueName = "order_placed";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _rabbitConnectionHelper.CreateConnectionWithRetryAsync();
            _channel = await _connection.CreateChannelAsync(null, stoppingToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 5, global: false, cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(message);

                    if (orderEvent != null)
                    {
                        await _emailSender.SendOrderConfirmationAsync(orderEvent);
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[OrderEmailConsumer] Error occurred");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true); // TODO: zamienić na retry/DLQ
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("OrderEmailConsumer started listening...");

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
