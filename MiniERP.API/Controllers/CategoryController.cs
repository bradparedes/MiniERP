using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Commands.Categories;
using MiniERP.Application.Queries.Categories;
using MiniERP.Application.Requests.Categories;

namespace MiniERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Obtener todas las categorías activas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Crear una nueva categoría
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var command = new CreateCategoryCommand(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Eliminar categoría (Soft Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound(new { message = "Category not found or could not be deleted." });

            return Ok(new { message = "Category successfully deleted." });
        }
    }
}
