using MediatR;
using MiniERP.Application.DTOs.Categories;

namespace MiniERP.Application.Queries.Categories;

public class GetCategoryByIdQuery : IRequest<CategoryResponse?>
{
    public int Id { get; }

    public GetCategoryByIdQuery(int id) => Id = id;
}