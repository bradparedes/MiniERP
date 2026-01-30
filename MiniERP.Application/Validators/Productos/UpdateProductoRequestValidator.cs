using FluentValidation;
using MiniERP.Application.DTOs.Productos;

namespace MiniERP.Application.Validators.Productos
{
    public class UpdateProductoRequestValidator : AbstractValidator<UpdateProductoRequest>
    {
        public UpdateProductoRequestValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("Debe especificarse una categoría válida.");
        }
    }
}