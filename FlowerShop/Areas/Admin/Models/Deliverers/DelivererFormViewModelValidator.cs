using FluentValidation;
using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public class DelivererFormViewModelValidator : AbstractValidator<DelivererFormViewModel>
{
    public DelivererFormViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ime je obavezno.")
            .MinimumLength(2).WithMessage("Ime mora imati najmanje 2 karaktera.")
            .MaximumLength(50).WithMessage("Ime ne sme imati više od 50 karaktera.")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Ime može sadržati samo slova.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Prezime je obavezno.")
            .MinimumLength(2).WithMessage("Prezime mora imati najmanje 2 karaktera.")
            .MaximumLength(50).WithMessage("Prezime ne sme imati više od 50 karaktera.")
            .Matches(@"^[\p{L}\s]+$").WithMessage("Prezime može sadržati samo slova.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresa je obavezna.")
            .EmailAddress().WithMessage("Unesite validnu email adresu.")
            .MaximumLength(100).WithMessage("Email adresa je predugačka.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Broj telefona je obavezan.")
            .Matches(@"^\+?(\d[\s-]?){7,15}$").WithMessage("Unesite validan broj telefona.");

        RuleFor(x => x.VehicleType)
            .IsInEnum().WithMessage("Tip vozila je obavezan.");

        RuleFor(x => x.DelivererStatus)
            .IsInEnum().WithMessage("Status dostavljača je obavezan.");
    }
}
