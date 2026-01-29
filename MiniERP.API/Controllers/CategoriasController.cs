using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.DTOs.Categorias;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Constants;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // OBTENER TODAS LAS CATEGORÍAS (User, Admin)
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _categoriaService.GetAllAsync();

            return Ok(new
            {
                message = "Lista de categorías",
                data = categorias
            });
        }

        // 🔓 User, Admin
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);

            if (categoria == null)
                return NotFound(new { message = "Categoría no encontrada" });

            return Ok(new
            {
                message = "Categoría encontrada",
                data = categoria
            });
        }

        // 🔐 Admin
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio" });

            try
            {
                var categoria = await _categoriaService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = categoria.Id },
                    new
                    {
                        message = "Categoría creada correctamente",
                        data = categoria
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔐 Admin
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio" });

            var updated = await _categoriaService.UpdateAsync(id, request);

            if (!updated)
                return NotFound(new { message = "Categoría no encontrada o inactiva" });

            return Ok(new { message = "Categoría actualizada correctamente" });
        }

        // 🔐 Admin (soft delete)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _categoriaService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Categoría no encontrada o ya eliminada" });

                return Ok(new { message = "Categoría eliminada correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
