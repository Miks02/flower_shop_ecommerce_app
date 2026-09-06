using FluentValidation;

namespace FlowerShop.Web.ViewModels;

public class CheckoutViewModelValidator : AbstractValidator<CheckoutViewModel>
{
    public CheckoutViewModelValidator()
    {
        RuleFor(x => x.RecipientFullName)
            .NotEmpty().WithMessage("Ime i prezime primaoca je obavezno.")
            .MinimumLength(3).WithMessage("Ime i prezime primaoca mora imati najmanje 3 karaktera.")
            .MaximumLength(150).WithMessage("Ime i prezime primaoca je predugačko.");

        RuleFor(x => x.RecipientPhoneNumber)
            .NotEmpty().WithMessage("Telefon primaoca je obavezan.")
            .Matches(@"^\+?(\d[\s-]?){7,15}$").WithMessage("Unesite validan broj telefona primaoca.");

        RuleFor(x => x.OrderAddress)
            .NotEmpty().WithMessage("Adresa dostave je obavezna.")
            .MinimumLength(5).WithMessage("Adresa dostave mora imati najmanje 5 karaktera.")
            .MaximumLength(250).WithMessage("Adresa dostave je predugačka.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Grad je obavezan.")
            .MinimumLength(2).WithMessage("Grad mora imati najmanje 2 karaktera.")
            .MaximumLength(100).WithMessage("Grad je predugačak.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Poštanski broj je obavezan.")
            .Matches(@"^[0-9A-Za-z\s-]{4,10}$").WithMessage("Unesite validan poštanski broj.");

        RuleFor(x => x.DeliveryDate)
            .NotNull().WithMessage("Datum dostave je obavezan.")
            .Must(d => d is null || d.Value >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Datum dostave ne može biti u prošlosti.");

        RuleFor(x => x.DeliveryTime)
            .NotNull().WithMessage("Željeno vreme dostave je obavezno.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Poruka na čestitki može imati maksimalno 500 karaktera.");

        RuleFor(x => x.BuyerEmail)
            .NotEmpty().WithMessage("Email adresa kupca je obavezna.")
            .EmailAddress().WithMessage("Unesite validnu email adresu.")
            .MaximumLength(100).WithMessage("Email adresa je predugačka.");

        RuleFor(x => x.BuyerPhone)
            .NotEmpty().WithMessage("Kontakt telefon kupca je obavezan.")
            .Matches(@"^\+?(\d[\s-]?){7,15}$").WithMessage("Unesite validan kontakt telefon kupca.");

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Broj kartice je obavezan.")
            .Matches(@"^(\d{4}[\s-]?){3}\d{4}$|^\d{15,19}$").WithMessage("Unesite validan broj kartice (16 cifara).");

        RuleFor(x => x.CardExpiry)
            .NotEmpty().WithMessage("Datum isteka kartice je obavezan.")
            .Matches(@"^(0[1-9]|1[0-2])\/([0-9]{2})$").WithMessage("Format isteka mora biti MM/YY.");

        RuleFor(x => x.CardCvc)
            .NotEmpty().WithMessage("CVC/CVV kod je obavezan.")
            .Matches(@"^[0-9]{3,4}$").WithMessage("CVC kod mora imati 3 ili 4 cifre.");

        RuleFor(x => x.CardHolder)
            .NotEmpty().WithMessage("Ime na kartici je obavezno.")
            .MinimumLength(3).WithMessage("Ime na kartici mora imati najmanje 3 karaktera.")
            .MaximumLength(100).WithMessage("Ime na kartici je predugačko.");

        RuleFor(x => x.AcceptTerms)
            .Equal(true).WithMessage("Morate prihvatiti uslove korišćenja.");
    }
}
