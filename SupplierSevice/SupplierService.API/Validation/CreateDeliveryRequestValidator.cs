using FluentValidation;
using SupplierService.Application.DTOs;

namespace SupplierService.API.Validation;

public sealed class CreateDeliveryRequestValidator : AbstractValidator<CreateDeliveryRequest>
{
    public CreateDeliveryRequestValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
