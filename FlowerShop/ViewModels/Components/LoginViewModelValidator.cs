using FluentValidation;

namespace FlowerShop.Web.ViewModels.Components;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Unesite email adresu");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Unesite lozinku");
    }
}
