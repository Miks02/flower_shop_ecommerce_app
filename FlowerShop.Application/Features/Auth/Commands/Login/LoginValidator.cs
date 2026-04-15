using FluentValidation;

namespace FlowerShop.Application.Features.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Unesite korisničko ime");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Unesite lozinku");
    }
}