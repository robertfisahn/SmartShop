using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SmartShopAPI.Tests.Helpers
{
    [CollectionDefinition("IntegrationTests", DisableParallelization = true)]
    public class IntegrationTestCollection : ICollectionFixture<WebApplicationFactory<Program>>
    {
    }
}
