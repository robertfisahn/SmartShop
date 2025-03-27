namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<int> GetUserRoleIdAsync(string roleName);
    }
}
