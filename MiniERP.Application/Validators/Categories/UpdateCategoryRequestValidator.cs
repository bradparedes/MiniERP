using FluentValidation;
using MiniERP.Application.Requests.Categories;

namespace MiniERP.Application.Validators.Categories;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}