using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.API.Validation;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.Role)
            .Must(r => string.IsNullOrWhiteSpace(r) || r is "User" or "Admin")
            .WithMessage("Role must be 'User' or 'Admin'.");
    }
}
