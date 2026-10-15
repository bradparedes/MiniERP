using MiniERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ICategoryService _categoryService;

        public CategoryController(IMediator mediator, ICategoryService categoryService)
        {
            _mediator = mediator;
            _categoryService = categoryService;
        }

        // Obtener todas las categorías activas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET /api/category/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            return result is null ? NotFound(new { message = "Category not found." }) 
            : Ok(result);
        }

        // PUT /api/category/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            var command = new UpdateCategoryCommand(id, request);
            var result = await _mediator.Send(command);
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
