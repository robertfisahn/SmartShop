using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using SmartShopAPI.Exceptions;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Tests.Helpers.Fixtures;

namespace SmartShopAPI.Tests.UnitTests.ServiceTests
{
    public class ProductServiceUnitTests(ProductServiceFixture fixture) : IClassFixture<ProductServiceFixture>
    {
        [Fact]
        public async Task GetAll_ReturnsAllProducts()
        {
            fixture.ResetProducts();
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
            fixture.ResetProducts();

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
            fixture.ResetProducts();
            var service = fixture.Service;
            var searchPhrase = "Product2";

            var result = await service.Search(searchPhrase);

            result.Should().NotBeEmpty();
            result[0].Should().BeOfType<ProductDto>();
        }


        [Fact]
        public async Task Search_ReturnsEmptyList_WhenNoMatchingProducts()
        {
            var service = fixture.Service;
            var searchPhrase = "NonExistentProduct";

            var result = await service.Search(searchPhrase);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsProductId()
        {
            fixture.ResetProducts();
            fixture.MockProductRepository.Invocations.Clear();

            fixture.MockProductRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback((Product p) =>
                {
                    var nextId = fixture.Products.Any() ? fixture.Products.Max(x => x.Id) + 1 : 1;
                    p.Id = nextId;
                    fixture.Products.Add(p);
                })
                .Returns(Task.CompletedTask);

            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "New Product", CategoryId = 1 };

            var result = await service.Create(dto, null);

            result.Should().Be(4);
            fixture.MockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task Create_WithoutImage_SetsDefaultImagePath()
        {
            fixture.ResetProducts();
            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "New Product", CategoryId = 1 };

            Product? addedProduct = null;

            fixture.MockProductRepository
                .Setup(r => r.ExistsByNameAsync(dto.Name, null))
                .ReturnsAsync(false);

            fixture.MockProductRepository
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback((Product p) => addedProduct = p)
                .Returns(Task.CompletedTask);

            var result = await service.Create(dto, null);

            addedProduct.Should().NotBeNull();
            addedProduct!.ImagePath.Should().Be("images/products/default.jpg");
        }




        [Fact]
        public async Task Create_WithImage_SavesImageAndSetImagePath()
        {
            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "New Product", CategoryId = 1 };
            var mockFile = new Mock<IFormFile>();
            var expectedImagePath = $"images/products/{mockFile.Object.FileName}";

            fixture.MockProductRepository
                .Setup(r => r.ExistsByNameAsync(dto.Name, null))
                .ReturnsAsync(false);

            fixture.MockFileService
                .Setup(fs => fs.SaveImageAsync(mockFile.Object))
                .ReturnsAsync(expectedImagePath);

            var result = await service.Create(dto, mockFile.Object);

            fixture.MockProductRepository.Verify(r => r.AddAsync(It.Is<Product>(p =>
                p.ImagePath == expectedImagePath)), Times.Once);
        }


        [Fact]
        public async Task Create_WithNonExistingCategory_ThrowsNotFoundException()
        {
            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "New Product", CategoryId = 999 };

            fixture.MockCategoryRepository.Setup(r => r.ExistsAsync(dto.CategoryId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => service.Create(dto, null));
            fixture.MockCategoryRepository.Verify(r => r.ExistsAsync(dto.CategoryId), Times.Once);
        }

        [Fact]
        public async Task Create_WithDuplicateName_ThrowsBadRequestException()
        {
            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "Product1", CategoryId = 1 };

            fixture.MockProductRepository.Setup(r => r.ExistsByNameAsync(dto.Name, null)).ReturnsAsync(true);

            await Assert.ThrowsAsync<BadRequestException>(() => service.Create(dto, null));
            fixture.MockProductRepository.Verify(r => r.ExistsByNameAsync(dto.Name, null), Times.Once);
        }

        [Fact]
        public async Task Delete_Product_Successfully()
        {
            fixture.ResetProducts();
            fixture.MockProductRepository.Invocations.Clear();
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
            var nonExistingProductId = 88;

            fixture.MockProductRepository.Setup(r => r.GetByIdAsync(nonExistingProductId)).ReturnsAsync((Product?)null);

            await FluentActions.Invoking(() => service.Delete(nonExistingProductId))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task Update_Product_Successfully()
        {
            fixture.ResetProducts();
            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "Updated product" };
            var product = fixture.Products.First(p => p.Id == 1);

            fixture.MockProductRepository.Invocations.Clear();
            fixture.MockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            await service.Update(1, dto, null);

            product.Name.Should().Be("Updated product");
            fixture.MockMapper.Verify(m => m.Map(dto, It.IsAny<Product>()), Times.Once);
            fixture.MockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_Product_NonExistingProduct_ThrowsNotFoundException()
        {
            var service = fixture.Service;
            var dto = new UpsertProductDto { Name = "Updated Product" };
            var nonExistingId = 99;

            fixture.MockProductRepository.Invocations.Clear();
            fixture.MockProductRepository.Setup(r => r.GetByIdAsync(nonExistingId)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => service.Update(nonExistingId, dto, null));
            fixture.MockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void PaginateProducts_PaginatesCorrectly()
        {
            var service = fixture.Service;
            fixture.Products.Add(new Product { Id = 4, Name = "Product4", Price = 100.00M, CategoryId = 1 });

            var paginated = service.Paginate(fixture.Products, 2, 2);

            Assert.Equal(2, paginated.Count);
            Assert.Equal("Product4", paginated[1].Name);
        }

        [Fact]
        public void SortProducts_ByNameAscending()
        {
            var service = fixture.Service;
            fixture.Products.Clear();
            fixture.Products.AddRange(
            [
                new Product { Name = "Product3", Price = 299.99M },
                new Product { Name = "Product2", Price = 199.99M },
                new Product { Name = "Product1", Price = 249.99M },
            ]);

            var sorted = service.Sort(fixture.Products, SortOrder.Ascending, "Name").ToList();

            Assert.Equal("Product1", sorted[0].Name);
            Assert.Equal("Product3", sorted[2].Name);
        }

        [Fact]
        public void SortProducts_ByPriceAscending()
        {
            var service = fixture.Service;
            fixture.Products.Clear();
            fixture.Products.AddRange(
            [
                new Product { Name = "Product3", Price = 299.99M },
                new Product { Name = "Product2", Price = 199.99M },
                new Product { Name = "Product1", Price = 249.99M },
            ]);

            var sorted = service.Sort(fixture.Products, SortOrder.Ascending, "Price").ToList();

            Assert.Equal("Product2", sorted[0].Name);
            Assert.Equal("Product3", sorted[2].Name);
        }

        [Fact]
        public void SortProducts_ByPriceDescending()
        {
            var service = fixture.Service;
            fixture.Products.Clear();
            fixture.Products.AddRange(
            [
                new Product { Name = "Product3", Price = 299.99M },
                new Product { Name = "Product2", Price = 199.99M },
                new Product { Name = "Product1", Price = 249.99M },
            ]);

            var sorted = service.Sort(fixture.Products, SortOrder.Descending, "Price").ToList();

            Assert.Equal("Product3", sorted[0].Name);
            Assert.Equal("Product2", sorted[2].Name);
        }

        [Fact]
        public void SortProducts_ByNameDescending()
        {
            var service = fixture.Service;
            fixture.Products.Clear();
            fixture.Products.AddRange(
            [
                new Product { Name = "Product3", Price = 299.99M },
                new Product { Name = "Product2", Price = 199.99M },
                new Product { Name = "Product1", Price = 249.99M },
            ]);

            var sorted = service.Sort(fixture.Products, SortOrder.Descending, "Name").ToList();

            Assert.Equal("Product3", sorted[0].Name);
            Assert.Equal("Product1", sorted[2].Name);
        }
    }
}
