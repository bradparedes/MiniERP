using MediatR;
using MiniERP.Application.Requests.Products;
using MiniERP.Application.DTOs.Products;

namespace MiniERP.Application.Commands.Products;

public class UpdateProductCommand : IRequest<ProductResponse>
{
    public int Id { get; }
    public UpdateProductRequest Request { get; }

    public UpdateProductCommand(int id, UpdateProductRequest request)
    {
        Id = id;
        Request = request;
    }
}