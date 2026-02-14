using FluentValidation;
using SharedKernel.ValueObjects;
using SupplierService.Application.DTOs;

namespace SupplierService.API.Validation;

public sealed class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .Must(BeValidCnpj)
            .WithMessage("Invalid CNPJ.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\d{8,20}$")
            .WithMessage("Phone must contain only digits (8-20 length).");
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
