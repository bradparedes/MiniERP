using FluentValidation;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Requests.Products;

namespace MiniERP.Application.Validators.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("A valid category must be specified.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("The product's active status must be specified.");
        }
    }
}