using Microsoft.AspNetCore.Mvc.Testing;

using Xunit;

namespace SmartShopAPI.Tests.Helpers
{
    [CollectionDefinition("IntegrationTests", DisableParallelization = true)]
    public class IntegrationTestCollection : ICollectionFixture<WebApplicationFactory<Program>>
    {
    }
}
