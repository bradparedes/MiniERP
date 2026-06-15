using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Application.Requests.Categories;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Constants;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategorysController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategorysController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // OBTENER TODAS LAS CATEGORÍAS (User, Admin)
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(new
            {
                message = "Lista de categorías",
                data = categories
            });
        }

        // 🔓 User, Admin
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { message = "Categoría no encontrada" });

            return Ok(new
            {
                message = "Categoría encontrada",
                data = category
            });
        }

        // 🔐 Admin
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            var category = await _categoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, new
            {
                message = "Categoría creada correctamente",
                data = category
            });
        }

        // 🔐 Admin
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            var updated = await _categoryService.UpdateAsync(id, request);

            if (!updated)
                return NotFound(new { message = "Categoría no encontrada o inactiva" });

            return Ok(new { message = "Categoría actualizada correctamente" });
        }

        // 🔐 Admin (soft delete)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Categoría no encontrada o ya eliminada" });

            return Ok(new { message = "Categoría eliminada correctamente" });
        }
    }
}
