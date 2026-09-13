using MediatR;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Application.Requests.Categories;

namespace MiniERP.Application.Commands.Categories
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryResponse>;
}
