using MassTransit;

using SmartShopAPI.Interfaces.Services.Integrations;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services.Infrastructure;

public class OrderEmailConsumer(IEmailSender emailSender, ILogger<OrderEmailConsumer> logger) : IConsumer<OrderPlacedEvent>
{
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<OrderEmailConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing order {OrderId}, sending email to {Email}",
            message.OrderId, message.Email);

        await _emailSender.SendOrderConfirmationAsync(message);

        _logger.LogInformation("Email sent for order {OrderId}", message.OrderId);
    }
}
