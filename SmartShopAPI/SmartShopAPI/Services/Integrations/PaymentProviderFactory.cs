using SmartShopAPI.Interfaces.Services.Integrations;

namespace SmartShopAPI.Services.Integrations
{
    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IEnumerable<IPaymentProvider> _providers;

        public PaymentProviderFactory(IEnumerable<IPaymentProvider> providers)
        {
            _providers = providers;
        }

        public IPaymentProvider GetProvider(string providerName)
        {
            var provider = _providers.FirstOrDefault(p =>
                p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
                throw new Exception($"Unknown payment provider: {providerName}");

            return provider;
        }
    }
}
