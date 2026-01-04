using SmartShopAPI.Interfaces.Services.Integrations;
using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Services.Integrations
{
    public class ResendEmailSender : IEmailSender
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public ResendEmailSender(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task SendOrderConfirmationAsync(OrderPlacedEvent message)
        {
            var apiKey = _config["Resend:ApiKey"];
            var from = _config["Resend:From"];

            var htmlBody = $@"
                <h1>Thank you for your order!</h1>
                <p>Your order ID: <strong>{message.OrderId}</strong></p>
                <p>Total: {message.TotalPrice} zł</p>
                <p>Products:</p>
                <ul>{string.Join("", message.ProductNames.Select(p => $"<li>{p}</li>"))}</ul>
            ";

            var payload = new
            {
                from,
                to = message.Email,
                subject = "SmartShop – Order Confirmation",
                html = htmlBody
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = JsonContent.Create(payload)
            };

            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _http.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }
    }
}
