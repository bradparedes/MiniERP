using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Interfaces;
using MiniERP.Application.DTOs.Productos;
using MiniERP.Core.Constants;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _productoService.GetAllAsync();

            return Ok(new
            {
                message = "Lista de productos",
                data = productos
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoRequest request)
        {
            if (id <= 0)
                return BadRequest(new { message = "El id debe ser mayor que cero." });

            
                var updated = await _productoService.UpdateAsync(id, request);

                return Ok(new
                {
                    message = "Producto actualizado correctamente",
                    data = updated
                });
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateProductoRequest request)
        {
                var producto = await _productoService.CreateAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = producto.Id }, new
                {
                    message = "Producto creado correctamente",
                    data = producto
                });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El id debe ser mayor que cero." });

            var deleted = await _productoService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Producto no encontrado o ya eliminado" });

            return Ok(new { message = "Producto eliminado correctamente" });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El id debe ser mayor que cero." });
            
            var producto = await _productoService.GetByIdAsync(id);

            if (producto == null)
                return NotFound(new { message = "Producto no encontrado o inactivo" });

            return Ok(new
            {
                message = "Producto encontrado",
                data = producto
            });
        }
    }
}
