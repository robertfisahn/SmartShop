using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IOrderService
    {
        int Create();
        Order GetById(int id);
    }
}