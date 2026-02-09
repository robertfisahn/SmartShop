using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Tests.Integration.Shared;
using SmartShopAPI.Tests.Integration.Shared.Helpers;
using SmartShopAPI.Entities;
using SmartShopAPI.Models.Enums;

namespace SmartShopAPI.Tests.Integration.Products;

public class ProductIntegrationTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetById_ReturnsOk()
    {
        var productId = 2;
        var response = await _client.GetAsync($"/api/product/{productId}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(1, 88)]
    [InlineData(88, 1)]
    public async Task GetById_ReturnsNotFound(int categoryId, int productId)
    {
        var response = await _client.GetAsync($"/api/category/{categoryId}/product/{productId}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        var categoryId = 1;
        var queryParams = ProductTestData.QueryParams;
        var queryString = queryParams.ToQueryString();
        var response = await _client.GetAsync($"/api/category/{categoryId}/product?{queryString}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ReturnsNotFound()
    {
        var invalidCategoryId = 88;
        var queryParams = ProductTestData.QueryParams;
        var queryString = queryParams.ToQueryString();
        var response = await _client.GetAsync($"/api/category/{invalidCategoryId}/product?{queryString}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(ProductTestData.InvalidQueryParamsList), MemberType = typeof(ProductTestData))]
    public async Task Get_InvalidQueryParams_ReturnsBadRequest(QueryParams queryParams)
    {
        var categoryId = 1;
        var queryString = queryParams.ToQueryString();
        var response = await _client.GetAsync($"/api/category/{categoryId}/product?{queryString}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenProductIsCreatedWithoutFile()
    {
        await ConfigureClientForAdminAsync();

        var content = ProductTestData.CreateProductMultipart;

        var response = await _client.PostAsync("/api/product", content);
        response.EnsureSuccessStatusCode();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Theory]
    [MemberData(nameof(ProductTestData.InvalidCreateProductMultiparts), MemberType = typeof(ProductTestData))]
    public async Task Create_InvalidProductData_ReturnsBadRequest(MultipartFormDataContent createProductMultipart)
    {
        await ConfigureClientForAdminAsync();
        var response = await _client.PostAsync("/api/product", createProductMultipart);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(false, System.Net.HttpStatusCode.Unauthorized)]
    [InlineData(true, System.Net.HttpStatusCode.Forbidden)]
    public async Task Create_ReturnsForbidden(bool isUser, System.Net.HttpStatusCode expectedStatusCode)
    {
        if (isUser)
        {
            await ConfigureClientForUserAsync();
        }

        var response = await _client.PostAsync("/api/product", ProductTestData.CreateProductMultipart);
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        await ConfigureClientForAdminAsync();
        var productId = 2;
        var response = await _client.DeleteAsync($"/api/product/{productId}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(2, false, System.Net.HttpStatusCode.Unauthorized)]
    [InlineData(2, true, System.Net.HttpStatusCode.Forbidden)]
    public async Task Delete_ReturnsForbidden(int productId, bool isUser, System.Net.HttpStatusCode expectedStatusCode)
    {
        if (isUser)
        {
            await ConfigureClientForUserAsync();
        }
        var response = await _client.DeleteAsync($"/api/product/{productId}");
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [InlineData(1, 88)]
    [InlineData(88, 1)]
    public async Task Delete_ReturnsNotFound(int categoryId, int productId)
    {
        await ConfigureClientForAdminAsync();
        var response = await _client.DeleteAsync($"/api/category/{categoryId}/product/{productId}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        await ConfigureClientForAdminAsync();
        var productId = 1;
        var response = await _client.PutAsync($"/api/product/{productId}", ProductTestData.UpdateProductMultipart);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest()
    {
        await ConfigureClientForAdminAsync();
        var productId = 1;
        var response = await _client.PutAsync($"/api/product/{productId}", ProductTestData.InvalidUpdateProductMultipart);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1, false, System.Net.HttpStatusCode.Unauthorized)]
    [InlineData(1, true, System.Net.HttpStatusCode.Forbidden)]
    public async Task Update_ReturnsUnauthorized(int productId, bool isUser, System.Net.HttpStatusCode expectedStatusCode)
    {
        if (isUser)
        {
            await ConfigureClientForUserAsync();
        }
        var response = await _client.PutAsync($"/api/product/{productId}", ProductTestData.UpdateProductMultipart);
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public async Task Update_ReturnsForbidden()
    {
        await ConfigureClientForUserAsync();
        var productId = 1;
        var response = await _client.PutAsync($"/api/product/{productId}", ProductTestData.UpdateProductMultipart);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_ReturnsNotFound()
    {
        await ConfigureClientForAdminAsync();
        var invalidProductId = 88;
        var response = await _client.PutAsync($"/api/product/{invalidProductId}", ProductTestData.UpdateProductMultipart);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
