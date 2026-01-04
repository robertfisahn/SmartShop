using System.Net.Http.Headers;
using System.Runtime;

using Microsoft.Extensions.Options;

using SmartShopAPI;
using SmartShopAPI.Models.Dtos.Payment;

public class PayPalProvider : IPaymentProvider
{
    public string Name => "paypal";
    private readonly PayPalSettings _config;
    private readonly HttpClient _http;

    public PayPalProvider(IOptions<PayPalSettings> config, HttpClient http)
    {
        _config = config.Value;
        _http = http;
    }

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

        var data = await response.Content.ReadFromJsonAsync<PayPalCreateOrderResponse>();

        return new PaymentInitResult
        {
            ProviderOrderId = data.id,
            PaymentUrl = data.links.First(x => x.rel == "approve").href
        };
    }

    public async Task<PaymentStatusResult> VerifyPaymentAsync(string providerOrderId)
    {
        var response = await _http.GetAsync($"{_config.BaseUrl}/v2/checkout/orders/{providerOrderId}");
        var data = await response.Content.ReadFromJsonAsync<PayPalCaptureResponse>();

        bool success = data.status == "COMPLETED";

        return new PaymentStatusResult
        {
            Success = success,
            ProviderOrderId = providerOrderId,
            OrderId = int.Parse(data.purchase_units[0].reference_id)
        };
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var authString = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{_config.ClientId}:{_config.Secret}")
        );

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        });

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadFromJsonAsync<PayPalAccessTokenResponse>();

        return json.access_token;
    }
}

public class PayPalAccessTokenResponse
{
    public string access_token { get; set; }
}
