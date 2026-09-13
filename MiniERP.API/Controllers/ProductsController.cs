using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Commands.Products;
using MiniERP.Application.Queries.Products;
using MiniERP.Application.Requests.Products;

namespace MiniERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 📦 Obtener todos los productos activos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // 🆕 Crear un nuevo producto
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var command = new CreateProductCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // 🗑️ Eliminar producto (Soft Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { message = "Product not found or could not be deleted." });

            return Ok(new { message = "Product successfully deleted." });
        }
    }
}
