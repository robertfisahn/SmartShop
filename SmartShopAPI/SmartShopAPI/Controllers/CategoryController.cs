using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Controllers
{
    [Route("api/category")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<CategoryDto>> GetAll()
        {
            var categories = _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<CategoryDto> GetCategory([FromRoute]int categoryId)
        {
            var category = _categoryService.GetCategory(categoryId);
            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public ActionResult Create([FromBody]CategoryUpsertDto dto) 
        {
            var categoryId = _categoryService.Create(dto);
            return Created($"category/{categoryId}", null);
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult Delete([FromRoute]int categoryId)
        {
            _categoryService.Delete(categoryId);
            return NoContent();
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult Update([FromRoute]int categoryId, [FromBody]CategoryUpsertDto dto)
        {
            _categoryService.Update(categoryId, dto);
            return Ok();
        }
    }
}
