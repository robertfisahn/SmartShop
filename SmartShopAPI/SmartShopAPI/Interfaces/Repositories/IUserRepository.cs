using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
        int? GetUserAddressId(int userId);
        void Add(User user);
        void SaveChanges();
    }
}
