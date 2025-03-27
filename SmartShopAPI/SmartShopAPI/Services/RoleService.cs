using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;

namespace SmartShopAPI.Services
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        public async Task<int> GetUserRoleIdAsync()
        {
            return await roleRepository.GetUserRoleIdAsync("User");
        }
    }
}
