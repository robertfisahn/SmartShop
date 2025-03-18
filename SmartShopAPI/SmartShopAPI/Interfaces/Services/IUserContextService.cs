using System.Security.Claims;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        public int GetUserId();
    }
}