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
            _logger.LogInformation("QoS set: prefetchCount={Prefetch}", 5);

            var mainQueueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", QueueName + "-dlq" }
            };

            _logger.LogInformation("Declaring main queue '{Queue}' with DLX→'{Dlq}'", QueueName, QueueName + "-dlq");
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainQueueArgs,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Declaring DLQ '{QueueDlq}'", QueueName + "-dlq");
            await _channel.QueueDeclareAsync(
                queue: QueueName + "-dlq",
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var deliveryTag = ea.DeliveryTag;
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogDebug("Message received: tag={Tag}, size={Size}B", deliveryTag, body.Length);

                    var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(message);

                    if (orderEvent != null)
                    {
                        _logger.LogInformation("Processing order event: tag={Tag}, orderId={OrderId}", deliveryTag, orderEvent.OrderId);
                        await _emailSender.SendOrderConfirmationAsync(orderEvent);
                        await _channel.BasicAckAsync(deliveryTag, multiple: false);
                        _logger.LogInformation("ACK sent: tag={Tag}", deliveryTag);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid payload (null after deserialize). Sending to DLQ. tag={Tag}", deliveryTag);
                        await _channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[OrderEmailConsumer] Error occurred while processing. tag={Tag}", deliveryTag);
                    await _channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false);
                    _logger.LogWarning("NACK (requeue:false) sent: tag={Tag} → DLQ", deliveryTag);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("OrderEmailConsumer started listening on '{Queue}'...", QueueName);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("OrderEmailConsumer stopping (cancellation requested).");
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
