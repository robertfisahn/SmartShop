using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Interfaces.Events
{
    public interface IEmailSender
    {
        Task SendOrderConfirmationAsync(OrderPlacedEvent orderEvent);
    }
}
