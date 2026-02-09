using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SmartShopAPI.Models.Dtos.Category;
using SmartShopAPI.Tests.Integration.Shared;
using SmartShopAPI.Entities;

namespace SmartShopAPI.Tests.Integration.Categories;

public class CategoryIntegrationTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/category");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/category/1");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<CategoryDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/category/999");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenAdmin()
    {
        await ConfigureClientForAdminAsync();
        var dto = CategoryTestData.ValidCategory;

        var response = await _client.PostAsJsonAsync("/api/category", dto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Theory]
    [MemberData(nameof(CategoryTestData.InvalidCategories), MemberType = typeof(CategoryTestData))]
    public async Task Create_ReturnsBadRequest_WhenInvalidData(CategoryUpsertDto invalidDto)
    {
        await ConfigureClientForAdminAsync();
        var response = await _client.PostAsJsonAsync("/api/category", invalidDto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(false, System.Net.HttpStatusCode.Unauthorized)]
    [InlineData(true, System.Net.HttpStatusCode.Forbidden)]
    public async Task Create_ReturnsForbidden(bool asUser, System.Net.HttpStatusCode expectedStatus)
    {
        if (asUser)
            await ConfigureClientForUserAsync();

        var response = await _client.PostAsJsonAsync("/api/category", CategoryTestData.ValidCategory);
        response.StatusCode.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenAdmin()
    {
        await ConfigureClientForAdminAsync();
        var dto = CategoryTestData.UpdateCategory;

        var response = await _client.PutAsJsonAsync("/api/category/1", dto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        await ConfigureClientForAdminAsync();
        var dto = CategoryTestData.UpdateCategory;

        var response = await _client.PutAsJsonAsync("/api/category/999", dto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenInvalidData()
    {
        await ConfigureClientForAdminAsync();
        var dto = CategoryTestData.InvalidCategory;

        var response = await _client.PutAsJsonAsync("/api/category/1", dto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenAdmin()
    {
        await ConfigureClientForAdminAsync();
        var response = await _client.DeleteAsync("/api/category/2");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenInvalidId()
    {
        await ConfigureClientForAdminAsync();
        var response = await _client.DeleteAsync("/api/category/999");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(false, System.Net.HttpStatusCode.Unauthorized)]
    [InlineData(true, System.Net.HttpStatusCode.Forbidden)]
    public async Task Delete_ReturnsForbiddenOrUnauthorized(bool asUser, System.Net.HttpStatusCode expectedStatus)
    {
        if (asUser)
            await ConfigureClientForUserAsync();

        var response = await _client.DeleteAsync("/api/category/2");
        response.StatusCode.Should().Be(expectedStatus);
    }
}
