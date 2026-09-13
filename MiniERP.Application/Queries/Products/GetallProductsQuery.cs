using System.Collections.Generic;
using MediatR;
using MiniERP.Application.DTOs.Products;

namespace MiniERP.Application.Queries.Products
{
    public record GetAllProductsQuery : IRequest<List<ProductResponse>>;
}
