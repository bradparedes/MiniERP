using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Products;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Exceptions;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, ct);
        if (product == null)
            throw new NotFoundException("Product not found.");

        // Validar categoría si cambia
        if (request.Request.CategoryId != product.CategoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Request.CategoryId, ct);
            if (category == null || !category.IsActive)
                throw new NotFoundException("Category not found or inactive.");
        }

        product.Name = request.Request.Name.Trim();
        product.Description = request.Request.Description?.Trim();
        product.Price = request.Request.Price;
        product.Stock = request.Request.Stock;
        product.CategoryId = request.Request.CategoryId;

        await _unitOfWork.Products.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }
}