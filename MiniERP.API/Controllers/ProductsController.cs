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

        [HttpGet("Get-All-Products")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(new
            {
                message = "Products list",
                data = products
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            if (id <= 0)
                return BadRequest(new { message = "the id must be greater than zero." });

            
                var updated = await _productService.UpdateAsync(id, request);

                return Ok(new
                {
                    message = "Successfully updated product",
                    data = updated
                });
        }

        [HttpPost("Create-Product")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
                var product = await _productService.CreateAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = product.Id }, new
                {
                    message = "Successfully created product",
                    data = product
                });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "the id must be greater than zero." });

            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Product not found or already deleted" });

            return Ok(new { message = "Product deleted successfully" });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = new GetProductByIdRequest { Id = id };

            if (request.Id <= 0)
                return BadRequest(new { message = "the id must be greater than zero." });
            
            var product = await _productService.GetByIdAsync(request.Id);

            if (product == null)
                return NotFound(new { message = "Product not found or inactive" });

            return Ok(new
            {
                message = "Product found",
                data = product
            });
        }
    }
}
