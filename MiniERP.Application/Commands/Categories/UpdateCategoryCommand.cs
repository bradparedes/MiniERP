using MediatR;
using MiniERP.Application.Requests.Categories;
using MiniERP.Application.DTOs.Categories;

namespace MiniERP.Application.Commands.Categories;

public class UpdateCategoryCommand : IRequest<CategoryResponse>
{
    public int Id { get; }
    public UpdateCategoryRequest Request { get; }

    public UpdateCategoryCommand(int id, UpdateCategoryRequest request)
    {
        Id = id;
        Request = request;
    }
}