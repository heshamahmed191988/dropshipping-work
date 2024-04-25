using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.GetallPigintaion(5, 0); 
           
          
                return Ok(result.Entities); // Assuming Entities holds the list of categories
           
                
           
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryDto category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _categoryService.Create(category);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(Get), new { id = category.Id }, category); // Adjust as necessary
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetById(id);
            if (category != null)
            {
                return Ok(category);
            }
            else
            {
                return NotFound();
            }
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryDto category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _categoryService.Update(category);
            if (result.IsSuccess)
            {
                return NoContent(); // 204 No Content for successful update without body
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.SoftDelete(id);
            if (result.IsSuccess)
            {
                return NoContent(); // 204 No Content on successful delete
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
 
