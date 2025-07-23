using SendGrid;
using SendGrid.Helpers.Mail;

using SmartShopAPI.Interfaces.Events;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services
{
    public class SendGridEmailSender(IConfiguration configuration) : IEmailSender
    {
        private readonly string _apiKey = configuration["SendGrid:ApiKey"]!;

        public async Task SendOrderConfirmationAsync(OrderPlacedEvent orderEvent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(
                configuration["SendGrid:FromEmail"],
                configuration["SendGrid:FromName"]
            );
            var to = new EmailAddress(orderEvent.Email);

            var subject = $"Order Confirmation #{orderEvent.OrderId}";
            var plainTextContent = $"Thank you for your order #{orderEvent.OrderId} totaling {orderEvent.TotalPrice} PLN.\nProducts: {string.Join(", ", orderEvent.ProductNames)}";
            var htmlContent = $"<strong>Thank you for your order #{orderEvent.OrderId} totaling {orderEvent.TotalPrice} PLN.</strong><br>Products: {string.Join(", ", orderEvent.ProductNames)}";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
