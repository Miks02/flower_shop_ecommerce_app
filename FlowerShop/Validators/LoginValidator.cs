using FluentValidation;
using FlowerShop.Web.ViewModels.Components;

namespace FlowerShop.Web.Validators;

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