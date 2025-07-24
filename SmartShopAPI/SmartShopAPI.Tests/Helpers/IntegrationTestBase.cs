using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SmartShopAPI.Data;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Tests.Helpers
{
    [Collection("IntegrationTests")]
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<SmartShopDbContext>));
                        if (dbContextOptions != null)
                        {
                            services.Remove(dbContextOptions);
                        }
                        services.AddDbContext<SmartShopDbContext>(options => options.UseInMemoryDatabase("SmartShopDb"));
                    });
                });
            _client = _factory.CreateClient();
        }

        private async Task<string> GetJwtTokenAsync(string email, string password)
        {
            var loginDto = new LoginDto { Email = email, Password = password };

            var response = await _client.PostAsJsonAsync("api/account/login", loginDto);
            response.EnsureSuccessStatusCode();


            var dto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return dto.Token;
        }

        protected async Task ConfigureClientForAdminAsync()
        {
            var token = await GetJwtTokenAsync("admin@admin.com", "admin123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task ConfigureClientForUserAsync()
        {
            var token = await GetJwtTokenAsync("user@user.com", "user1234");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
