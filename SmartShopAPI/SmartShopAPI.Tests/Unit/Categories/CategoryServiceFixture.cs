using AutoMapper;
using Moq;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models.Dtos.Category;
using SmartShopAPI.Services.Core;

namespace SmartShopAPI.Tests.Unit.Categories;

public class CategoryServiceFixture
{
    public Mock<ICategoryRepository> MockCategoryRepository { get; }
    public Mock<IMapper> MockMapper { get; }
    public List<Category> Categories { get; }
    public CategoryService Service { get; }

    public CategoryServiceFixture()
    {
        MockCategoryRepository = new Mock<ICategoryRepository>();
        MockMapper = new Mock<IMapper>();

        Categories = new List<Category>
        {
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Clothing" },
            new() { Id = 3, Name = "Books" }
        };

        SetupRepositories();
        SetupMapper();

        Service = new CategoryService(MockCategoryRepository.Object, MockMapper.Object);
    }

    private void SetupRepositories()
    {
        MockCategoryRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(Categories);

        MockCategoryRepository.Setup(r => r.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => Categories.FirstOrDefault(c => c.Id == id));

        MockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .Callback((Category c) =>
            {
                var nextId = Categories.Any() ? Categories.Max(x => x.Id) + 1 : 1;
                c.Id = nextId;
                Categories.Add(c);
            })
            .Returns(Task.CompletedTask);

        MockCategoryRepository.Setup(r => r.Delete(It.IsAny<Category>()))
            .Callback((Category c) => Categories.Remove(c));

        MockCategoryRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
    }

    private void SetupMapper()
    {
        MockMapper.Setup(m => m.Map<List<CategoryDto>>(It.IsAny<List<Category>>()))
            .Returns((List<Category> src) => src.Select(s => new CategoryDto { Id = s.Id, Name = s.Name }).ToList());

        MockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns((Category src) => new CategoryDto { Id = src.Id, Name = src.Name });

        MockMapper.Setup(m => m.Map<Category>(It.IsAny<CategoryUpsertDto>()))
            .Returns((CategoryUpsertDto dto) => new Category { Name = dto.Name });

        MockMapper.Setup(m => m.Map(It.IsAny<CategoryUpsertDto>(), It.IsAny<Category>()))
            .Callback((CategoryUpsertDto dto, Category category) => category.Name = dto.Name);
    }

    public void Reset()
    {
        Categories.Clear();
        Categories.AddRange(new List<Category>
        {
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Clothing" },
            new() { Id = 3, Name = "Books" }
        });

        MockCategoryRepository.Invocations.Clear();
        MockMapper.Invocations.Clear();

        MockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(Categories);
        MockCategoryRepository.Setup(r => r.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => Categories.FirstOrDefault(c => c.Id == id));
    }
}
