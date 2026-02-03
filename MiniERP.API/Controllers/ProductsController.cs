using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Interfaces;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Requests.Products;
using MiniERP.Core.Constants;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(new
            {
                message = "Lista de productos",
                data = products
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            if (id <= 0)
                return BadRequest(new { message = "El id debe ser mayor que cero." });

            
                var updated = await _productService.UpdateAsync(id, request);

                return Ok(new
                {
                    message = "Producto actualizado correctamente",
                    data = updated
                });
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
                var product = await _productService.CreateAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = product.Id }, new
                {
                    message = "Producto creado correctamente",
                    data = product
                });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El id debe ser mayor que cero." });

            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Producto no encontrado o ya eliminado" });

            return Ok(new { message = "Producto eliminado correctamente" });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = new GetProductByIdRequest { Id = id };

            if (request.Id <= 0)
                return BadRequest(new { message = "El id debe ser mayor que cero." });
            
            var product = await _productService.GetByIdAsync(request.Id);

            if (product == null)
                return NotFound(new { message = "Producto no encontrado o inactivo" });

            return Ok(new
            {
                message = "Producto encontrado",
                data = product
            });
        }
    }
}
