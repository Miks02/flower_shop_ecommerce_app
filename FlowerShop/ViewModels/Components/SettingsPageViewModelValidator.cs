using FluentValidation;

namespace FlowerShop.Web.ViewModels.Components;

public class SettingsPageViewModelValidator : AbstractValidator<SettingsPageViewModel>
{
    public SettingsPageViewModelValidator()
    {
        RuleFor(p => p.ProfileVm.FirstName)
            .NotEmpty().WithMessage("Ime je obavezno.")
            .MinimumLength(3).WithMessage("Ime mora imati najmanje 3 karaktera.")
            .MaximumLength(20).WithMessage("Ime može imati maksimalno 20 karaktera.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Ime može sadržati samo slova.");

        RuleFor(p => p.ProfileVm.LastName)
            .NotEmpty().WithMessage("Prezime je obavezno.")
            .MinimumLength(3).WithMessage("Prezime mora imati najmanje 3 karaktera.")
            .MaximumLength(30).WithMessage("Prezime može imati maksimalno 30 karaktera.")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Prezime može sadržati samo slova.");
        
        RuleFor(p => p.ProfileVm.Email)
            .NotEmpty().WithMessage("Email adresa je obavezna.")
            .EmailAddress().WithMessage("Unesite validnu email adresu.")
            .MaximumLength(50).WithMessage("Email adresa je predugačka.");

        RuleFor(p => p.ProfileVm.PhoneNumber)
            .NotEmpty().WithMessage("Broj telefona je obavezan.")
            .Matches(@"^\+?(\d[\s-]?){7,15}$").WithMessage("Unesite validan broj telefona.");

        RuleFor(p => p.ProfileVm.ProfilePicture)
            .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
            .WithMessage("Maksimalna dužina fajla je 5 MB.")
            .Must(file => file == null || IsSupportedContentType(file.ContentType))
            .WithMessage("Dozvoljeni formati su: JPG, JPEG i PNG.");

        When(p => !string.IsNullOrEmpty(p.ChangePasswordVm.CurrentPassword), () =>
        {
            RuleFor(p => p.ChangePasswordVm.NewPassword)
                .NotEmpty().WithMessage("Lozinka je obavezna.")
                .MinimumLength(8).WithMessage("Lozinka mora imati najmanje 8 karaktera.");

            RuleFor(p => p.ChangePasswordVm.ConfirmPassword)
                .NotEmpty().WithMessage("Potvrda lozinke je obavezna.")
                .Equal(p => p.ChangePasswordVm.NewPassword).WithMessage("Lozinke se ne poklapaju.");
        });
    }

    private bool IsSupportedContentType(string contentType)
    {
        return contentType.Equals("image/jpg") ||
               contentType.Equals("image/jpeg") ||
               contentType.Equals("image/png");
    }
}
