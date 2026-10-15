using MediatR;
using MiniERP.Application.Queries.Categories;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Categories;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, ct);
        if (category == null || !category.IsActive) return null;

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}