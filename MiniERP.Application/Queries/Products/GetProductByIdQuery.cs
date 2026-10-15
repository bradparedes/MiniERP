using MediatR;
using MiniERP.Application.DTOs.Products;

namespace MiniERP.Application.Queries.Products;

public class GetProductByIdQuery : IRequest<ProductResponse?>
{
    public int Id { get; }
    public GetProductByIdQuery(int id) => Id = id;
}