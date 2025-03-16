using Microsoft.AspNetCore.Authorization;

namespace SmartShopAPI.Authorization
{
    public enum ResourceOperation
    {
        Update,
        Delete
    }

    public class ResourceOperationRequirement(ResourceOperation resourceOperation) : IAuthorizationRequirement
    {
        public ResourceOperation ResourceOperation { get; } = resourceOperation;
    }
}
