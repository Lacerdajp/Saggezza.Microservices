using FluentValidation;
using SharedKernel.ValueObjects;
using SupplierService.Application.DTOs;

namespace SupplierService.API.Validation;

public sealed class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateSupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(150)
            .When(x => x.Name is not null);

        RuleFor(x => x.Cnpj)
            .Must(BeValidCnpj)
            .WithMessage("Invalid CNPJ.")
            .When(x => x.Cnpj is not null);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => x.Email is not null);

        RuleFor(x => x.Phone)
            .Matches(@"^\d{8,20}$")
            .WithMessage("Phone must contain only digits (8-20 length).")
            .When(x => x.Phone is not null);
    }

    private static bool BeValidCnpj(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            _ = Cnpj.Create(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
