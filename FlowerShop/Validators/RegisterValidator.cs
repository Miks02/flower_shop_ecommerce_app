using FluentValidation;
using FlowerShop.ViewModels.Components; 

namespace FlowerShop.Validators;

public class RegisterValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterValidator()
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
        
        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage("Korisničko ime je obavezno.")
            .MinimumLength(5).WithMessage("Korisničko ime mora imati najmanje 5 karaktera.")
            .MaximumLength(25).WithMessage("Korisničko ime može imati maksimalno 25 karaktera.")
            .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Korisničko ime može sadržati samo slova, brojeve, tačku (.) i donju crtu (_)."); 

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email adresa je obavezna.")
            .EmailAddress().WithMessage("Unesite validnu email adresu.")
            .MaximumLength(50).WithMessage("Email adresa je predugačka.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati najmanje 8 karaktera.")
            .Matches(@"[A-Z]+").WithMessage("Lozinka mora sadržati barem jedno veliko slovo.")
            .Matches(@"[a-z]+").WithMessage("Lozinka mora sadržati barem jedno malo slovo.")
            .Matches(@"[0-9]+").WithMessage("Lozinka mora sadržati barem jedan broj.");


        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("Potvrda lozinke je obavezna.")
            .Equal(p => p.Password).WithMessage("Lozinke se ne poklapaju."); 


        RuleFor(p => p.PhoneNumber)
            .NotEmpty().WithMessage("Broj telefona je obavezan.")
            .Matches(@"^\+?(\d[\s-]?){7,15}$").WithMessage("Unesite validan broj telefona."); 
    }
}