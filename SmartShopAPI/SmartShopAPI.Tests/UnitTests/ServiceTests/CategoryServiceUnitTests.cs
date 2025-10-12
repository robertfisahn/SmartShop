using FluentAssertions;

using Moq;

using SmartShopAPI.Exceptions;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;
using SmartShopAPI.Tests.UnitTests.Fixtures;

namespace SmartShopAPI.Tests.UnitTests.ServiceTests
{
    public class CategoryServiceUnitTests(CategoryServiceFixture fixture) : IClassFixture<CategoryServiceFixture>
    {
        [Fact]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            var service = fixture.Service;
            var result = await service.GetAll();

            result.Should().HaveCount(3);
            result[0].Name.Should().Be("Electronics");
            fixture.MockCategoryRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCategoryExists_ShouldReturnMappedCategory()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            var service = fixture.Service;
            var result = await service.GetById(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Electronics");
            fixture.MockCategoryRepository.Verify(r => r.GetAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            fixture.MockCategoryRepository.Setup(r => r.GetAsync(99)).ReturnsAsync((Category?)null);

            await FluentActions.Invoking(() => fixture.Service.GetById(99))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Category not found");
        }

        [Fact]
        public async Task Create_ShouldAddCategoryAndReturnId()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            var service = fixture.Service;
            var dto = new CategoryUpsertDto { Name = "New Category" };

            var result = await service.Create(dto);

            result.Should().Be(4);
            fixture.MockCategoryRepository.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "New Category")), Times.Once);
            fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Delete_WhenCategoryExists_ShouldRemoveAndSave()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            var service = fixture.Service;
            var existing = fixture.Categories.First();

            await service.Delete(existing.Id);

            fixture.MockCategoryRepository.Verify(r => r.Delete(It.Is<Category>(c => c.Id == existing.Id)), Times.Once);
            fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Delete_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            fixture.MockCategoryRepository.Setup(r => r.GetAsync(999)).ReturnsAsync((Category?)null);

            await FluentActions.Invoking(() => fixture.Service.Delete(999))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Category not found");

            fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_WhenCategoryExists_ShouldMapAndSave()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            var service = fixture.Service;
            var dto = new CategoryUpsertDto { Name = "Updated Category" };
            var existing = fixture.Categories.First();

            await service.Update(existing.Id, dto);

            existing.Name.Should().Be("Updated Category");
            fixture.MockMapper.Verify(m => m.Map(dto, existing), Times.Once);
            fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Update_WhenCategoryDoesNotExist_ShouldThrowNotFoundException()
        {
            fixture.ResetCategories();
            fixture.MockCategoryRepository.Invocations.Clear();

            fixture.MockCategoryRepository.Setup(r => r.GetAsync(777)).ReturnsAsync((Category?)null);

            var dto = new CategoryUpsertDto { Name = "Doesn't matter" };

            await FluentActions.Invoking(() => fixture.Service.Update(777, dto))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Category not found");

            fixture.MockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
