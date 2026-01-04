using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Core;

namespace SmartShopAPI.Services.Core
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        public async Task<int> GetUserRoleIdAsync()
        {
            return await roleRepository.GetUserRoleIdAsync("User");
        }
    }
}
