using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using SmartShopAPI.Interfaces.Events;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class OrderEmailConsumer(IEmailSender emailSender, ILogger<OrderEmailConsumer> logger) : BackgroundService
    {
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<OrderEmailConsumer> _logger = logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string HostName = "localhost";
        private const string QueueName = "order_placed";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(null, stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: false,
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
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[OrderEmailConsumer] Error occurred");
                }
            };


            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: true,
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
