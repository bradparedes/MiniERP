using MediatR;

namespace MiniERP.Application.Commands.Products
{
    public record DeleteProductCommand(int ProductId) : IRequest<bool>;
}
