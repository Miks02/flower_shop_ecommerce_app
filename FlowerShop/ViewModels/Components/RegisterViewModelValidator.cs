using FluentValidation;

namespace FlowerShop.Web.ViewModels.Components;

public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterViewModelValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("Ime je obavezno.")
            .MinimumLength(3).WithMessage("Ime mora imati najmanje 3 karaktera.")
            .MaximumLength(20).WithMessage("Ime može imati maksimalno 20 karaktera.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Ime može sadržati samo slova.");

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("Prezime je obavezno.")
            .MinimumLength(3).WithMessage("Prezime mora imati najmanje 3 karaktera.")
            .MaximumLength(30).WithMessage("Prezime može imati maksimalno 30 karaktera.")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Prezime može sadržati samo slova.");
        
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email adresa je obavezna.")
            .EmailAddress().WithMessage("Unesite validnu email adresu.")
            .MaximumLength(50).WithMessage("Email adresa je predugačka.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(6).WithMessage("Lozinka mora imati najmanje 6 karaktera.");

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("Potvrda lozinke je obavezna.")
            .Equal(p => p.Password).WithMessage("Lozinke se ne poklapaju.");

        RuleFor(p => p.PhoneNumber)
            .NotEmpty().WithMessage("Broj telefona je obavezan.")
            .Matches(@"^\+?(\d[\s-]?){7,15}$").WithMessage("Unesite validan broj telefona.");
    }
}
