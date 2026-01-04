namespace SmartShopAPI.Interfaces.Services.Integrations
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetProvider(string providerName);
    }
}
