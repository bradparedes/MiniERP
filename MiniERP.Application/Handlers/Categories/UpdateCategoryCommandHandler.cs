using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Categories;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Application.Exceptions;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Categories;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, ct);
        if (category == null)
            throw new NotFoundException("Category not found.");

        // Validar duplicado (case-insensitive) excluyendo la actual
        var allCategories = await _unitOfWork.Categories.GetAllAsync();
        var duplicate = allCategories.Any(c => c.IsActive && c.Id != request.Id &&
            c.Name.Trim().Equals(request.Request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

        if (duplicate)
            throw new ConflictException($"Category '{request.Request.Name}' already exists.");

        category.Name = request.Request.Name.Trim();
        category.Description = request.Request.Description?.Trim();

        await _unitOfWork.Categories.UpdateAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

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