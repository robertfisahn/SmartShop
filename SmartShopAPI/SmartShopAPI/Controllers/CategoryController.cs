using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.Category;
using System.Threading.Tasks;

namespace SmartShopAPI.Controllers
{
    [Route("api/category")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            return Ok(await categoryService.GetAll());
        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CategoryDto>> GetCategory([FromRoute]int categoryId)
        {
            return Ok(await categoryService.GetById(categoryId));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult> Create([FromBody]CategoryUpsertDto dto) 
        {
            var categoryId = await categoryService.Create(dto);
            return Created($"category/{categoryId}", null);
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete([FromRoute]int categoryId)
        {
            await categoryService.Delete(categoryId);
            return NoContent();
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Update([FromRoute]int categoryId, [FromBody]CategoryUpsertDto dto)
        {
            await categoryService.Update(categoryId, dto);
            return Ok();
        }
    }
}
