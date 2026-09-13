using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Categories;
using MiniERP.Application.Queries.Categories;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Application.Interfaces;
namespace MiniERP.Application.Handlers.Categories
{
    // Handler para Crear Categoría
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<CategoryResponse> Handle(CreateCategoryCommand command, CancellationToken ct)
        {
            // Delegamos la creación al servicio de forma limpia
            return await _categoryService.CreateAsync(command.Request);
        }
    }

    // Handler para Listar Categorías
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponse>>
    {
        private readonly ICategoryService _categoryService;

        public GetAllCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<List<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
        {
            // Delegamos la consulta al servicio
            return await _categoryService.GetAllAsync();
        }
    }

    // Handler para Eliminar Categoría (Soft Delete)
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken ct)
        {
            // Delegamos la eliminación al servicio
            return await _categoryService.DeleteAsync(command.CategoryId);
        }
    }
}
