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
        private const int Prefetch = 5;
        private const int RetryTtlMs = 10_000;
        private const int MaxRetries = 3;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _rabbitConnectionHelper.CreateConnectionWithRetryAsync();
            _channel = await _connection.CreateChannelAsync(null, stoppingToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: (ushort)Prefetch, global: false, cancellationToken: stoppingToken);
            _logger.LogInformation("QoS set: prefetch={Prefetch}", Prefetch);

            // MAIN queue → DLX → RETRY
            var mainArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", QueueName + "-retry" }
            };
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainArgs,
                cancellationToken: stoppingToken);
            _logger.LogInformation("Declared main queue '{Queue}' (DLX → {Retry})", QueueName, QueueName + "-retry");

            // RETRY queue with TTL → DLX → MAIN
            var retryArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", QueueName },
                { "x-message-ttl", RetryTtlMs }
            };
            await _channel.QueueDeclareAsync(
                queue: QueueName + "-retry",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: retryArgs,
                cancellationToken: stoppingToken);
            _logger.LogInformation("Declared retry queue '{QueueRetry}' (TTL={Ttl}ms → DLX {Main})",
                QueueName + "-retry", RetryTtlMs, QueueName);

            // DLQ: final parking
            await _channel.QueueDeclareAsync(
                queue: QueueName + "-dlq",
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);
            _logger.LogInformation("Declared DLQ '{QueueDlq}'", QueueName + "-dlq");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var tag = ea.DeliveryTag;

                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(json);

                    if (orderEvent != null)
                    {
                        _logger.LogInformation("Processing order: tag={Tag}, orderId={OrderId}", tag, orderEvent.OrderId);
                        await _emailSender.SendOrderConfirmationAsync(orderEvent);
                        await _channel.BasicAckAsync(tag, multiple: false);
                        _logger.LogInformation("ACK: tag={Tag}", tag);
                    }
                    else
                    {
                        // Invalid payload → send to DLQ and ACK original
                        _logger.LogWarning("Invalid payload → DLQ. tag={Tag}", tag);
                        await PublishToDlqAsync(_channel!, QueueName, ea.Body, ea.BasicProperties);
                        await _channel.BasicAckAsync(tag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Processing error. tag={Tag}", tag);

                    var retries = GetRetryCount(ea);
                    if (retries >= MaxRetries)
                    {
                        _logger.LogWarning("Max retries reached ({Retries}). Parking to DLQ. tag={Tag}", retries, tag);
                        await PublishToDlqAsync(_channel!, QueueName, ea.Body, ea.BasicProperties);
                        await _channel.BasicAckAsync(tag, multiple: false);
                    }
                    else
                    {
                        _logger.LogInformation("Retry attempt {Attempt}/{Max}. NACK → retry. tag={Tag}", retries + 1, MaxRetries, tag);
                        await _channel.BasicNackAsync(tag, multiple: false, requeue: false);
                    }
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("OrderEmailConsumer listening on '{Queue}'...", QueueName);

            try { await Task.Delay(Timeout.Infinite, stoppingToken); }
            catch (TaskCanceledException) { _logger.LogInformation("OrderEmailConsumer stopping."); }
        }

        // --- helpers ---
        private static int GetRetryCount(BasicDeliverEventArgs ea)
        {
            try
            {
                if (ea.BasicProperties?.Headers is null) return 0;
                if (!ea.BasicProperties.Headers.TryGetValue("x-death", out var death) || death is not IList<object> deaths)
                    return 0;

                long total = 0;
                foreach (var item in deaths)
                {
                    if (item is IDictionary<string, object> dict &&
                        dict.TryGetValue("count", out var cnt))
                    {
                        total += cnt switch { long l => l, int i => i, _ => 0 };
                    }
                }
                return (int)total;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Publishes a message to the DLQ.  
        /// Creates new BasicProperties and optionally copies headers from the source message.  
        /// Adds an error timestamp for debugging/tracking.  
        /// </summary>
        private static async Task PublishToDlqAsync(
            IChannel ch,
            string queueName,
            ReadOnlyMemory<byte> body,
            IReadOnlyBasicProperties? sourceProps = null)
        {
            var props = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                Headers = new Dictionary<string, object?>()
            };

            if (sourceProps?.Headers is { } hdrs)
            {
                foreach (var kv in hdrs)
                    props.Headers[kv.Key] = kv.Value;
            }

            props.Headers["x-error-timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await ch.BasicPublishAsync(
                exchange: "",
                routingKey: queueName + "-dlq",
                mandatory: false,
                basicProperties: props,
                body: body);
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
