using System.Security.Claims;

using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Services.Infrastructure;

namespace SmartShopAPI.Services.Infrastructure
{
    public class UserContextService(IHttpContextAccessor contextAccessor) : IUserContextService
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public ClaimsPrincipal User => _contextAccessor.HttpContext?.User;

        public int GetUserId()
        {
            var claim = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                throw new NotFoundException("User not found");
            }
            return int.Parse(claim.Value);
        }
    }

}
