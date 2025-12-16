namespace SmartShopAPI.Models.Dtos.Payment;

using System.Text.Json.Serialization;

public class PayPalCaptureResponse
{
    public string id { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;

    [JsonPropertyName("purchase_units")]
    public List<PayPalPurchaseUnit> purchase_units { get; set; } = new();
}

public class PayPalPurchaseUnit
{
    [JsonPropertyName("reference_id")]
    public string reference_id { get; set; } = string.Empty;

    [JsonPropertyName("payments")]
    public PayPalPayments payments { get; set; } = new();
}

public class PayPalPayments
{
    public List<PayPalCapture> captures { get; set; } = new();
}

public class PayPalCapture
{
    public string status { get; set; } = string.Empty;
    public PayPalAmount amount { get; set; } = new();
}

public class PayPalAmount
{
    public string currency_code { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
}
