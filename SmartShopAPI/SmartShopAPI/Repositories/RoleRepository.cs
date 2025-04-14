using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Data;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class RoleRepository(SmartShopDbContext context) : IRoleRepository
    {
        public async Task<int> GetUserRoleIdAsync(string roleName) =>
            await context.Roles
                .Where(r => r.Name == roleName)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
    }
}
