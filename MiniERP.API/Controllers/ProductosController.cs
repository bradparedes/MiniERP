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
        [Authorize(Roles = Roles.Admin + "," + Roles.User)]
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
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateProductoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _productoService.UpdateAsync(id, request);
            
            if (!updated)
                return NotFound(new { message = "Producto no encontrado" });

            return Ok(new { message = "Producto actualizado correctamente" });
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateProductoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var producto = await _productoService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetAll),
                new {id = producto.Id },
                producto
            );
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productoService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Producto no encontrado" });

            return Ok(new { message = "Producto eliminado correctamente" });
        }
    }
}
