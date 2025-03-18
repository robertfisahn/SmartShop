using Microsoft.AspNetCore.Authorization;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Interfaces.Services;

namespace SmartShopAPI.Authorization
{
    public class ResourceOperationRequirementHandler(IUserContextService userContextService) : 
        AuthorizationHandler<ResourceOperationRequirement, IUserVerification>
    {
        private readonly IUserContextService _userContextService = userContextService;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement,
            IUserVerification resource)
        {
            var userId = _userContextService.GetUserId();
            
            if(resource is IEnumerable<IUserVerification> resourcesToVerify)
            {
                if(resourcesToVerify.First().UserId == userId)
                {
                    context.Succeed(requirement);
                }
            }
            else if(resource is IUserVerification resourceToVerify)
            {
                if (resourceToVerify.UserId == userId)
                {
                    context.Succeed(requirement);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
