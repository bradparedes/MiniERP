using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        // Obtener todos los productos activos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            return result is null ? NotFound(new { message = "Product not found." })
            : Ok(result);
        }
        // Crear un nuevo producto
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var command = new CreateProductCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        // Actualizar producto
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            var command = new UpdateProductCommand(id, request);
            var result = await _mediator.Send(command);
            return result is null ? NotFound(new { message = "Product not found." }) 
            : Ok(result);
        }

        // Eliminar producto (Soft Delete)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
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
