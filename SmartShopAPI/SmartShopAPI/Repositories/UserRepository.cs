using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class UserRepository(SmartShopDbContext context) : IUserRepository
    {
        public User? GetByEmail(string email) => context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
        public int? GetUserAddressId(int userId) => context.Users.Where(u => u.Id == userId).Select(u => u.AddressId).SingleOrDefault();
        public void Add(User user) => context.Users.Add(user);
        public void SaveChanges() => context.SaveChanges();
    }
}
