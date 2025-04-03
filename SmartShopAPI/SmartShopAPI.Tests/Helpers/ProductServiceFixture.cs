using AutoMapper;
using Moq;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Models;
using SmartShopAPI.Services;
using Microsoft.AspNetCore.Http;

namespace SmartShopAPI.Tests.Helpers
{
    public class ProductServiceFixture
    {
        public Mock<IProductRepository> MockProductRepository { get; }
        public Mock<ICategoryRepository> MockCategoryRepository { get; }
        public Mock<IFileService> MockFileService { get; }
        public Mock<IMapper> MockMapper { get; }
        public ProductService Service { get; }

        private readonly List<Product> _products;
        private readonly List<Category> _categories;

        public List<Product> Products => _products;
        public List<Category> Categories => _categories;

        public ProductServiceFixture()
        {
            MockProductRepository = new Mock<IProductRepository>();
            MockCategoryRepository = new Mock<ICategoryRepository>();
            MockFileService = new Mock<IFileService>();
            MockMapper = new Mock<IMapper>();

            _products = new List<Product>
            {
                new() { Id = 1, Name = "Product1", Price = 199.99M, CategoryId = 2, ImagePath = "images/products/default.jpg" },
                new() { Id = 2, Name = "Product2", Price = 149.99M, CategoryId = 1, ImagePath = "images/products/default.jpg" },
                new() { Id = 3, Name = "Product3", Price = 100.00M, CategoryId = 1, ImagePath = "images/products/default.jpg" }
            };

            _categories = new List<Category>
            {
                new() { Id = 1, Name = "Category1" },
                new() { Id = 2, Name = "Category2" }
            };

            SetupFileService();
            SetupRepositories();
            SetupMappers();

            Service = new ProductService(
                MockProductRepository.Object,
                MockMapper.Object,
                MockCategoryRepository.Object,
                MockFileService.Object
            );
        }

        private void SetupMappers()
        {
            MockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                .Returns((List<Product> sources) =>
                    sources.Select(s => new ProductDto { Id = s.Id, Name = s.Name }).ToList());

            MockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product source) => new ProductDto { Id = source.Id, Name = source.Name });

            MockMapper.Setup(m => m.Map<Product>(It.IsAny<UpsertProductDto>()))
                .Returns((UpsertProductDto dto) => new Product { Name = dto.Name, CategoryId = dto.CategoryId });

            MockMapper.Setup(m => m.Map(It.IsAny<UpsertProductDto>(), It.IsAny<Product>()))
                .Callback((UpsertProductDto dto, Product product) => product.Name = dto.Name);
        }

        private void SetupRepositories()
        {
            MockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(_categories);

            MockCategoryRepository.Setup(r => r.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _categories.Any(c => c.Id == id));

            MockProductRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_products);

            MockProductRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync((string name, int? id) =>
                    _products.Any(p => p.Name == name && (!id.HasValue || p.Id != id.Value)));

            MockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _products.FirstOrDefault(p => p.Id == id));

            MockProductRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback((Product p) =>
                {
                    p.Id = _products.Any() ? _products.Max(x => x.Id) + 1 : 1;
                    _products.Add(p);
                })
                .Returns(Task.CompletedTask);

            MockProductRepository.Setup(r => r.Delete(It.IsAny<Product>()))
                .Callback((Product p) => _products.Remove(p));

            MockProductRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            MockProductRepository
                .Setup(r => r.GetBySearchPhraseAsync(It.IsAny<string>()))
                .ReturnsAsync((string phrase) =>
                    _products.Where(p =>
                        p.Name.Contains(phrase, StringComparison.OrdinalIgnoreCase)).ToList());
        }

        private void SetupFileService()
        {
            MockFileService.Setup(fs => fs.SaveImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((IFormFile file) =>
                    file != null ? $"images/products/{file.FileName}" : "images/products/default.jpg");
        }

        public void ResetProducts()
        {

            _products.Clear();
            _products.AddRange(new List<Product>
            {
                new() { Id = 1, Name = "Product1", Price = 199.99M, CategoryId = 2, ImagePath = "images/products/default.jpg" },
                new() { Id = 2, Name = "Product2", Price = 149.99M, CategoryId = 1, ImagePath = "images/products/default.jpg" },
                new() { Id = 3, Name = "Product3", Price = 100.00M, CategoryId = 1, ImagePath = "images/products/default.jpg" }
            });
            MockProductRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(_products);
            MockProductRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _products.FirstOrDefault(p => p.Id == id));
        }
    }
}
