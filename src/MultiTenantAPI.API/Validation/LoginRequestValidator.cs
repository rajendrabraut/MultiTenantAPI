using FluentValidation;
using MultiTenantAPI.Application.Auth;

namespace MultiTenantAPI.API.Validation;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CompanyCode).NotEmpty().MaximumLength(50);
    }
}
