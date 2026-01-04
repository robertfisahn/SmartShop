using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Interfaces.Services.Integrations
{
    public interface IEmailSender
    {
        Task SendOrderConfirmationAsync(OrderPlacedEvent orderEvent);
    }
}
