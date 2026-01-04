using System.Security.Claims;

namespace SmartShopAPI.Interfaces.Services.Infrastructure
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        public int GetUserId();
    }
}
