using FluentValidation;

namespace FlowerShop.Web.ViewModels.Components;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator()
    {
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Unesite korisničko ime");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Unesite lozinku");
    }
}
