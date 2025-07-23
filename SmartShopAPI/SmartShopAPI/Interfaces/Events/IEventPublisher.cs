using SmartShopAPI.Models.Events;

namespace SmartShopAPI.Interfaces.Events;

public interface IEventPublisher
{
    Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent);
}
