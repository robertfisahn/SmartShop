using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.Product;

namespace SmartShopAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("product/all")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("product")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery]string searchPhrase)
        {
            var products = await _productService.GetProductsAsync(searchPhrase);
            return Ok(products);
        }

        [HttpGet("product/check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CheckName([FromQuery]string productName)
        {
            await _productService.CheckUniqueNameAsync(productName, null);
            return Ok(new { message = "Product name is available." });
        }

        [HttpGet("category/{categoryId}/product")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get([FromRoute]int categoryId, [FromQuery]QueryParams query)
        {
            var products = await _productService.GetAsync(categoryId, query);
            return Ok(products);
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductDto>> GetById([FromRoute]int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            return Ok(product);
        }

        [HttpPost("product")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Create([FromForm]UpsertProductDto dto, IFormFile? file)
        {
            var productId = await _productService.CreateAsync(dto, file);
            return Created($"api/product/{productId}", null);
        }

        [HttpDelete("product/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete([FromRoute]int productId) 
        {
            await _productService.DeleteAsync(productId);
            return NoContent();
        }

        [HttpPut("product/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Update([FromRoute]int productId, [FromForm]UpsertProductDto dto, IFormFile? file)
        {
            await _productService.UpdateAsync(productId, dto, file);
            return NoContent();
        }
    }
}
