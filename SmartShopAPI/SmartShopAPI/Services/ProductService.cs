using AutoMapper;
using SmartShopAPI.Data;
using SmartShopAPI.Models;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Interfaces.Services;

namespace SmartShopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly SmartShopDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(SmartShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.Include(c => c.Category).ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<List<ProductDto>> GetProductsAsync(string searchPhrase)
        {
            var products = await _context.Products.Where(p => p.Name.ToLower().Contains(searchPhrase.ToLower())).ToListAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<PagedResult<ProductDto>> GetAsync(int categoryId, QueryParams query)
        {
            await CheckCategory(categoryId);

            var filteredProducts = await FilterProducts(categoryId, query.SearchPhrase);
            var paginatedAndSortedProducts = PaginateProducts(SortProducts(filteredProducts, query.SortOrder, query.SortBy),
                query.PageSize, query.PageNumber);

            var dtos = _mapper.Map<List<ProductDto>>(paginatedAndSortedProducts);
            return new PagedResult<ProductDto>(dtos, filteredProducts.Count(), query.PageSize, query.PageNumber);
        }

        public async Task<ProductDto> GetByIdAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == productId) ?? throw new NotFoundException("Product not found");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<int> CreateAsync(UpsertProductDto dto, IFormFile? file)
        {
            await CheckCategory(dto.CategoryId);
            await CheckUniqueNameAsync(dto.Name, null);

            var product = _mapper.Map<Product>(dto);
            product.ImagePath = file != null ? await SaveImageAsync(file) : "images/products/default.jpg";

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new NotFoundException("Product not found");
            if (!IsDefaultImage(product.ImagePath))
            {
                DeleteFile(product.ImagePath!);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int productId, UpsertProductDto dto, IFormFile? file)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == productId) ?? throw new NotFoundException("Product not found");
            
            await CheckUniqueNameAsync(dto.Name, productId);
            product.UpdatedDate = DateTime.Now;
            if (file != null)
            {
                await UpdateProductImageAsync(product, dto, file);
            }
            _mapper.Map(dto, product);
            await _context.SaveChangesAsync();
        }

        private bool IsDefaultImage(string? imagePath)
        {
            var defaultPath = "images/products/default.jpg";
            return imagePath == defaultPath;
        }

        private async Task UpdateProductImageAsync(Product product, UpsertProductDto dto, IFormFile file)
        {
            if (!IsDefaultImage(product.ImagePath))
            {
                DeleteFile(product.ImagePath!);
            }

            dto.ImagePath = await SaveImageAsync(file);
        }

        public void DeleteFile(string imagePath)
        {
            var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
            if (File.Exists(fullImagePath))
            {
                File.Delete(fullImagePath);
            }
        }

        public async Task CheckUniqueNameAsync(string productName, int? productId)
        {
            bool productExists = await _context.Products
                .AnyAsync(p => p.Name == productName && (productId == null || p.Id != productId));
            if (productExists)
            {
                throw new BadRequestException("Product with the same name already exists.");
            }
        }

        public async Task CheckCategory(int categoryId)
        {
            if(!await _context.Categories.AnyAsync(x => x.Id == categoryId))
            {
                throw new NotFoundException("Category not found");
            }
        }

        public async Task<List<Product>> FilterProducts(int categoryId, string? searchPhrase)
        {
            var products = await _context.Products
                .Where(x => x.CategoryId == categoryId && (searchPhrase == null || x.Name.ToLower().Contains(searchPhrase.ToLower())))
                .ToListAsync();
            return products;
        }

        public List<Product> SortProducts(List<Product> products, SortOrder sortOrder, string sortBy)
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

        public List<Product> PaginateProducts(List<Product> products, int pageSize, int pageNumber)
        {
            var result = products
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize).ToList();
            return result;
        }

        public async Task<string?> SaveImageAsync(IFormFile file)
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
    }
}
