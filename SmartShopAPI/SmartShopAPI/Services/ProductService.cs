using AutoMapper;
using SmartShopAPI.Models;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Services
{
    public class ProductService(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository) : IProductService
    {
        public async Task<List<ProductDto>> GetAll()
        {
            return mapper.Map<List<ProductDto>>(await productRepository.GetAllAsync());
        }

        public async Task<List<ProductDto>> Search(string searchPhrase)
        {
            return mapper.Map<List<ProductDto>>(await productRepository.GetBySearchPhraseAsync(searchPhrase));
        }

        public async Task<PagedResult<ProductDto>> GetByCategory(int categoryId, QueryParams query)
        {
            await EnsureCategoryExists(categoryId);

            var filteredProducts = await productRepository.GetByCategoryAndSearchPhraseAsync(categoryId, query.SearchPhrase);
            var paginatedAndSortedProducts = Paginate(Sort(filteredProducts, query.SortOrder, query.SortBy),
                query.PageSize, query.PageNumber);

            var dtos = mapper.Map<List<ProductDto>>(paginatedAndSortedProducts);
            return new PagedResult<ProductDto>(dtos, filteredProducts.Count(), query.PageSize, query.PageNumber);
        }

        public async Task<ProductDto> GetById(int productId)
        {
            return mapper.Map<ProductDto>(await productRepository.GetByIdAsync(productId));
        }

        public async Task<int> Create(UpsertProductDto dto, IFormFile? file)
        {
            await EnsureCategoryExists(dto.CategoryId);
            await EnsureUniqueName(dto.Name, null);

            var product = mapper.Map<Product>(dto);
            product.ImagePath = file != null ? await SaveImage(file) : "images/products/default.jpg";

            await productRepository.AddAsync(product);
            await productRepository.SaveChangesAsync();
            return product.Id;
        }

        public async Task EnsureUniqueName(string productName, int? productId)
        {
            if (await productRepository.ExistsByNameAsync(productName, productId))
            {
                throw new BadRequestException("Product with the same name already exists.");
            }
        }

        private async Task EnsureCategoryExists(int categoryId)
        {
            if (!await categoryRepository.ExistsAsync(categoryId))
            {
                throw new NotFoundException("Category not found");
            }
        }

        public async Task<string?> SaveImage(IFormFile file)
        {
            var folderPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("wwwroot/images/products", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"images/products/{fileName}";
        }

        public async Task Delete(int productId)
        {
            var product = await productRepository.GetByIdAsync(productId) ?? throw new NotFoundException("Product not found");
            if (!IsDefaultImage(product.ImagePath))
            {
                RemoveImage(product.ImagePath!);
            }
            productRepository.Delete(product);
            await productRepository.SaveChangesAsync();
        }

        public async Task Update(int productId, UpsertProductDto dto, IFormFile? file)
        {
            var product = await productRepository.GetByIdAsync(productId) ?? throw new NotFoundException("Product not found");

            await EnsureUniqueName(dto.Name, productId);
            product.UpdatedDate = DateTime.Now;
            if (file != null)
            {
                if (!IsDefaultImage(product.ImagePath))
                {
                    RemoveImage(product.ImagePath!);
                }
                product.ImagePath = await SaveImage(file);
            }
            mapper.Map(dto, product);
            await productRepository.SaveChangesAsync();
        }

        private bool IsDefaultImage(string? imagePath)
        {
            var defaultPath = "images/products/default.jpg";
            return imagePath == defaultPath;
        }

        public void RemoveImage(string imagePath)
        {
            var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
            if (File.Exists(fullImagePath))
            {
                File.Delete(fullImagePath);
            }
        }

        public List<Product> Sort(List<Product> products, SortOrder sortOrder, string sortBy)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                var columnSelector = new Dictionary<string, Func<Product, object>>
                {
                    { nameof(Product.Name), r => r.Name },
                    { nameof(Product.Price), r => r.Price }
                };

                var selectedColumn = columnSelector[sortBy];

                products = sortOrder == SortOrder.Ascending
                    ? products.OrderBy(selectedColumn).ToList()
                    : products.OrderByDescending(selectedColumn).ToList();
            }
            return products;
        }

        public List<Product> Paginate(List<Product> products, int pageSize, int pageNumber)
        {
            var result = products
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize).ToList();
            return result;
        }

        public async Task UpdateStock(List<OrderItem> orderItems) => await productRepository.UpdateStockQuantity(orderItems);
    }
}
