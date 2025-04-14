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
    public class ProductController(IProductService productService) : ControllerBase
    {

        [HttpGet("product/all")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            return Ok(await productService.GetAll());
        }

        [HttpGet("product")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string searchPhrase)
        {
            return Ok(await productService.Search(searchPhrase));
        }

        [HttpGet("product/check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CheckIfNameIsAvailable([FromQuery] string productName)
        {
            await productService.EnsureUniqueName(productName, null);
            return Ok(new { message = "Product name is available." });
        }

        [HttpGet("category/{categoryId}/product")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory([FromRoute] int categoryId, [FromQuery] QueryParams query)
        {
            return Ok(await productService.GetByCategory(categoryId, query));
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductDto>> GetById([FromRoute] int productId)
        {
            return Ok(await productService.GetById(productId));
        }

        [HttpPost("product")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Create([FromForm] UpsertProductDto dto, IFormFile? file)
        {
            var productId = await productService.Create(dto, file);
            return Created($"api/product/{productId}", null);
        }

        [HttpDelete("product/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete([FromRoute] int productId)
        {
            await productService.Delete(productId);
            return NoContent();
        }

        [HttpPut("product/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Update([FromRoute] int productId, [FromForm] UpsertProductDto dto, IFormFile? file)
        {
            await productService.Update(productId, dto, file);
            return NoContent();
        }
    }
}
