using FluentAssertions;
using Moq;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Tests.Unit.Categories;

public class CategoryServiceTests : IClassFixture<CategoryServiceFixture>
{
    private readonly CategoryServiceFixture _fixture;

    public CategoryServiceTests(CategoryServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllCategories()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();

        // Act
        var result = await _fixture.Service.GetAll();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Electronics");
        _fixture.MockCategoryRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCategoryExists_ShouldReturnMappedCategory()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();

        // Act
        var result = await _fixture.Service.GetById(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Electronics");
        _fixture.MockCategoryRepository.Verify(r => r.GetAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();
        _fixture.MockCategoryRepository.Setup(r => r.GetAsync(99)).ReturnsAsync((Category?)null);

        // Act & Assert
        await FluentActions.Invoking(() => _fixture.Service.GetById(99))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");
    }

    [Fact]
    public async Task Create_ShouldAddCategoryAndReturnId()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();
        var dto = new CategoryUpsertDto { Name = "New Category" };

        // Act
        var result = await _fixture.Service.Create(dto);

        // Assert
        result.Should().Be(4);
        _fixture.MockCategoryRepository.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "New Category")), Times.Once);
        _fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Delete_WhenCategoryExists_ShouldRemoveAndSave()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();
        var existing = _fixture.Categories.First();

        // Act
        await _fixture.Service.Delete(existing.Id);

        // Assert
        _fixture.MockCategoryRepository.Verify(r => r.Delete(It.Is<Category>(c => c.Id == existing.Id)), Times.Once);
        _fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Delete_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();
        _fixture.MockCategoryRepository.Setup(r => r.GetAsync(999)).ReturnsAsync((Category?)null);

        // Act & Assert
        await FluentActions.Invoking(() => _fixture.Service.Delete(999))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");

        _fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Update_WhenCategoryExists_ShouldMapAndSave()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();
        var dto = new CategoryUpsertDto { Name = "Updated Category" };
        var existing = _fixture.Categories.First();

        // Act
        await _fixture.Service.Update(existing.Id, dto);

        // Assert
        existing.Name.Should().Be("Updated Category");
        _fixture.MockMapper.Verify(m => m.Map(dto, existing), Times.Once);
        _fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Update_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _fixture.Reset();
        _fixture.MockCategoryRepository.Invocations.Clear();
        _fixture.MockCategoryRepository.Setup(r => r.GetAsync(777)).ReturnsAsync((Category?)null);
        var dto = new CategoryUpsertDto { Name = "Doesn't matter" };

        // Act & Assert
        await FluentActions.Invoking(() => _fixture.Service.Update(777, dto))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");

        _fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
