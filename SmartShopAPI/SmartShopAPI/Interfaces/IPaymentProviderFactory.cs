namespace SmartShopAPI.Interfaces
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetProvider(string providerName);
    }
}
