using FluentValidation;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Requests.Products;

namespace MiniERP.Application.Validators.Products
{
    public class GetProductByIdRequestValidator : AbstractValidator<GetProductByIdRequest>
    {
        public GetProductByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El id del producto debe ser mayor a 0.");
        }
    }
}