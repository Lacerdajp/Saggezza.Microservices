using FluentValidation;
using SupplierService.Application.DTOs;

namespace SupplierService.API.Validation;

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(150)
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Sku)
            .MaximumLength(50)
            .When(x => x.Sku is not null);
    }
}
