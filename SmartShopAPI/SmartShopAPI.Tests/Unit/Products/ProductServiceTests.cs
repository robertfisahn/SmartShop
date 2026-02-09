using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Models.Enums;

namespace SmartShopAPI.Tests.Unit.Products;

public class ProductServiceTests(ProductServiceFixture fixture) : IClassFixture<ProductServiceFixture>
{
    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        fixture.Reset();
        var service = fixture.Service;

        var products = await service.GetAll();

        products.Should().NotBeEmpty();
        products.Should().HaveCount(3);
        products[0].Name.Should().Be("Product1");
    }

    [Fact]
    public async Task GetAll_WhenNoProducts_ReturnsEmptyList()
    {
        var service = fixture.Service;
        fixture.MockProductRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        var products = await service.GetAll();

        products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ExistingProduct()
    {
        fixture.Reset();
        var service = fixture.Service;

        var existingProduct = await service.GetById(1);

        existingProduct.Should().NotBeNull();
        existingProduct.Id.Should().Be(1);
        existingProduct.Name.Should().Be("Product1");
    }

    [Fact]
    public async Task GetById_NonExistingProduct_ThrowsNotFoundException_WithCorrectMessage()
    {
        var service = fixture.Service;
        var nonExistingProductId = 88;
        fixture.MockProductRepository.Setup(r => r.GetByIdAsync(nonExistingProductId)).ReturnsAsync((Product?)null);

        await FluentActions.Invoking(() => service.GetById(nonExistingProductId))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Product not found");
    }

    [Fact]
    public async Task Search_WithMatchingPhrase_ReturnsNonEmptyListOfProductDto()
    {
        fixture.Reset();
        var service = fixture.Service;
        var searchPhrase = "Product2";

        var result = await service.Search(searchPhrase);

        result.Should().NotBeEmpty();
        result[0].Should().BeOfType<ProductDto>();
    }

    [Fact]
    public async Task Search_ReturnsEmptyList_WhenNoMatchingProducts()
    {
        fixture.Reset();
        var service = fixture.Service;
        var searchPhrase = "NonExistentProduct";
        fixture.MockProductRepository.Setup(r => r.GetBySearchPhraseAsync(searchPhrase)).ReturnsAsync(new List<Product>());

        var result = await service.Search(searchPhrase);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsProductId()
    {
        fixture.Reset();
        fixture.MockProductRepository.Invocations.Clear();
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "New Product", CategoryId = 1 };

        var result = await service.Create(dto, null);

        result.Should().Be(4);
        fixture.MockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_WithoutImage_SetsDefaultImagePath()
    {
        fixture.Reset();
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "New Product Default", CategoryId = 1 };
        Product? addedProduct = null;

        fixture.MockProductRepository.Setup(r => r.ExistsByNameAsync(dto.Name, null)).ReturnsAsync(false);
        fixture.MockProductRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback((Product p) => addedProduct = p)
            .Returns(Task.CompletedTask);

        await service.Create(dto, null);

        addedProduct.Should().NotBeNull();
        addedProduct!.ImagePath.Should().Be("images/products/default.jpg");
    }

    [Fact]
    public async Task Create_WithImage_SavesImageAndSetImagePath()
    {
        fixture.Reset();
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "New Product with Image", CategoryId = 1 };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.jpg");
        var expectedImagePath = "images/products/test.jpg";

        fixture.MockProductRepository.Setup(r => r.ExistsByNameAsync(dto.Name, null)).ReturnsAsync(false);
        fixture.MockFileService.Setup(fs => fs.SaveImageAsync(mockFile.Object)).ReturnsAsync(expectedImagePath);

        await service.Create(dto, mockFile.Object);

        fixture.MockProductRepository.Verify(r => r.AddAsync(It.Is<Product>(p => p.ImagePath == expectedImagePath)), Times.Once);
    }

    [Fact]
    public async Task Create_WithNonExistingCategory_ThrowsNotFoundException()
    {
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "New Product", CategoryId = 999 };
        fixture.MockCategoryRepository.Setup(r => r.ExistsAsync(dto.CategoryId)).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Create(dto, null));
    }

    [Fact]
    public async Task Create_WithDuplicateName_ThrowsBadRequestException()
    {
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "Product1", CategoryId = 1 };
        fixture.MockProductRepository.Setup(r => r.ExistsByNameAsync(dto.Name, null)).ReturnsAsync(true);

        await Assert.ThrowsAsync<BadRequestException>(() => service.Create(dto, null));
    }

    [Fact]
    public async Task Delete_Product_Successfully()
    {
        fixture.Reset();
        var service = fixture.Service;
        var product = fixture.Products.First(p => p.Id == 1);
        fixture.MockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        await service.Delete(1);

        fixture.MockProductRepository.Verify(r => r.Delete(It.Is<Product>(p => p.Id == 1)), Times.Once);
        fixture.MockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_Product_NonExistingProduct_ThrowsNotFoundException()
    {
        var service = fixture.Service;
        fixture.MockProductRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        await FluentActions.Invoking(() => service.Delete(99))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Update_Product_Successfully()
    {
        fixture.Reset();
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "Updated product" };
        var product = fixture.Products.First(p => p.Id == 1);
        fixture.MockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        await service.Update(1, dto, null);

        product.Name.Should().Be("Updated product");
        fixture.MockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_Product_NonExistingProduct_ThrowsNotFoundException()
    {
        var service = fixture.Service;
        var dto = new UpsertProductDto { Name = "Updated Product" };
        fixture.MockProductRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Update(99, dto, null));
    }

    [Fact]
    public void Paginate_PaginatesCorrectly()
    {
        var service = fixture.Service;
        fixture.Reset();
        fixture.Products.Add(new Product { Id = 4, Name = "Product4", Price = 100.00M, CategoryId = 1 });

        var paginated = service.Paginate(fixture.Products, 2, 2);

        paginated.Should().HaveCount(2);
        paginated[1].Name.Should().Be("Product4");
    }

    [Fact]
    public void Sort_ByNameAscending()
    {
        var service = fixture.Service;
        fixture.Reset();
        var sorted = service.Sort(fixture.Products, SortOrder.Ascending, "Name").ToList();

        sorted[0].Name.Should().Be("Product1");
        sorted[2].Name.Should().Be("Product3");
    }

    [Fact]
    public void Sort_ByPriceAscending()
    {
        var service = fixture.Service;
        fixture.Reset();
        var sorted = service.Sort(fixture.Products, SortOrder.Ascending, "Price").ToList();

        sorted[0].Name.Should().Be("Product3"); // 100
        sorted[2].Name.Should().Be("Product1"); // 199
    }

    [Fact]
    public void Sort_ByPriceDescending()
    {
        var service = fixture.Service;
        fixture.Reset();
        var sorted = service.Sort(fixture.Products, SortOrder.Descending, "Price").ToList();

        sorted[0].Name.Should().Be("Product1"); // 199
        sorted[2].Name.Should().Be("Product3"); // 100
    }

    [Fact]
    public void Sort_ByNameDescending()
    {
        var service = fixture.Service;
        fixture.Reset();
        var sorted = service.Sort(fixture.Products, SortOrder.Descending, "Name").ToList();

        sorted[0].Name.Should().Be("Product3");
        sorted[2].Name.Should().Be("Product1");
    }
}
