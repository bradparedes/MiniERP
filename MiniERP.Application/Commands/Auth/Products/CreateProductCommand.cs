using MediatR;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Requests.Products;

namespace MiniERP.Application.Commands.Products
{
    public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductResponse>;
}
