using System.Collections.Generic;
using MediatR;
using MiniERP.Application.DTOs.Categories;

namespace MiniERP.Application.Queries.Categories
{
    public record GetAllCategoriesQuery : IRequest<List<CategoryResponse>>;
}
