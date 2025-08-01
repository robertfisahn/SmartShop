using RabbitMQ.Client;

namespace SmartShopAPI.Helpers
{
    public class RabbitMqConnectionHelper(RabbitMqSettings _settings, ILogger<RabbitMqConnectionHelper> _logger)
    {
        public async Task<IConnection> CreateConnectionWithRetryAsync(int maxRetries = 3, int delaySeconds = 5)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host ?? "localhost",
                Port = _settings.Port > 0 ? _settings.Port : 5672,
                UserName = _settings.UserName ?? "guest",
                Password = _settings.Password ?? "guest"
            };

            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    _logger.LogInformation("Attempt {i}/{maxRetries} to connect to RabbitMQ...", i, maxRetries);
                    var connection = await factory.CreateConnectionAsync();
                    _logger.LogInformation("RabbitMQ connection established.");
                    return connection;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Connection attempt {i} failed.", i);
                    if (i == maxRetries) throw;
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            throw new Exception("RabbitMQ connection failed after all retries.");
        }
    }
}
