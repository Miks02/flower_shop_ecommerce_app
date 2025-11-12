using FluentValidation;
using FlowerShop.ViewModels.Components;

namespace FlowerShop.Validators;

public class LoginValidator : AbstractValidator<LoginViewModel>
{
    public LoginValidator()
    {
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Unesite korisničko ime");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Unesite lozinku");
        
    }
}