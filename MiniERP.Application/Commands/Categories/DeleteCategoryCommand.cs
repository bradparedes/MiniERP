using MediatR;

namespace MiniERP.Application.Commands.Categories
{
    public record DeleteCategoryCommand(int CategoryId) : IRequest<bool>;
}
