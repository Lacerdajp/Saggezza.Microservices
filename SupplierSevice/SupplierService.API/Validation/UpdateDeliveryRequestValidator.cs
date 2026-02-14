using FluentValidation;
using SupplierService.Application.DTOs;

namespace SupplierService.API.Validation;

public sealed class UpdateDeliveryRequestValidator : AbstractValidator<UpdateDeliveryRequest>
{
    public UpdateDeliveryRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue);

        RuleFor(x => x)
            .Must(x => x.Quantity.HasValue || x.Status.HasValue)
            .WithMessage("At least one field (Quantity or Status) must be provided.");
    }
}
