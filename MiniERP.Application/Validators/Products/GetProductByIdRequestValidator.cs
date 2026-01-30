using FluentValidation;
using MiniERP.Application.DTOs.Productos;

namespace MiniERP.Application.Validators.Productos
{
    public class GetProductByIdRequestValidator : AbstractValidator<GetProductByIdRequest>
    {
        public GetProductByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El id debe ser mayor que 0.");
        }
    }
}