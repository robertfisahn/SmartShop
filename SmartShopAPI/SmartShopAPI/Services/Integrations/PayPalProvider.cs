using System.Net.Http.Headers;

using Microsoft.Extensions.Options;

using SmartShopAPI.Exceptions;
using SmartShopAPI.Models.Dtos.Payment;

namespace SmartShopAPI.Services.Integrations;

public class PayPalProvider(IOptions<PayPalSettings> config, HttpClient http) : IPaymentProvider
{
    public string Name => "paypal";
    private readonly PayPalSettings _config = config.Value;
    private readonly HttpClient _http = http;

    public async Task<PaymentInitResult> CreatePaymentAsync(int orderId, decimal amount)
    {
        var token = await GetAccessTokenAsync();

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.PostAsJsonAsync(
            $"{_config.BaseUrl}/v2/checkout/orders",
            new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = orderId.ToString(),
                        amount = new { currency_code = "USD", value = amount }
                    }
                },
                application_context = new
                {
                    return_url = _config.ReturnUrl,
                    cancel_url = _config.CancelUrl
                }
            });

        if (!response.IsSuccessStatusCode)
            throw new PaymentException($"PayPal API error: {response.StatusCode}");

        var data = await response.Content.ReadFromJsonAsync<PayPalCreateOrderResponse>()
            ?? throw new PaymentException("Failed to deserialize PayPal response");

        var approveLink = data.links?.FirstOrDefault(x => x.rel == "approve")
            ?? throw new PaymentException("PayPal response missing approve link");

        return new PaymentInitResult
        {
            ProviderOrderId = data.id,
            PaymentUrl = approveLink.href
        };
    }

    public async Task<PaymentStatusResult> VerifyPaymentAsync(string paymentReference)
    {
        var response = await _http.GetAsync($"{_config.BaseUrl}/v2/checkout/orders/{paymentReference}");

        if (!response.IsSuccessStatusCode)
            throw new PaymentException($"PayPal verification failed: {response.StatusCode}");

        var data = await response.Content.ReadFromJsonAsync<PayPalCaptureResponse>()
            ?? throw new PaymentException("Failed to deserialize PayPal verification response");

        return new PaymentStatusResult
        {
            Success = data.status == "COMPLETED",
            ProviderOrderId = paymentReference,
            OrderId = int.Parse(data.purchase_units[0].reference_id)
        };
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var authString = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{_config.ClientId}:{_config.Secret}")
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        });

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new PaymentException($"Failed to get PayPal access token: {response.StatusCode}");

        var json = await response.Content.ReadFromJsonAsync<PayPalAccessTokenResponse>()
            ?? throw new PaymentException("Failed to deserialize PayPal token response");

        return json.access_token;
    }

    private class PayPalAccessTokenResponse
    {
        public string access_token { get; set; } = null!;
    }
}
