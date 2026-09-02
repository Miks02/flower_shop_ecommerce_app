using FluentValidation;

namespace FlowerShop.Web.Areas.Admin.Models.Flowers;

public class AddFlowerViewModelValidator : AbstractValidator<AddFlowerViewModel>
{
    public AddFlowerViewModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Naziv cveta je obavezan.")
            .MaximumLength(50)
            .WithMessage("Naziv cveta ne sme imati više od 50 karaktera.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Boja cveta je obavezna.")
            .MaximumLength(30)
            .WithMessage("Boja cveta ne sme imati više od 30 karaktera.");

        RuleFor(x => x.FlowerCategory)
            .IsInEnum()
            .WithMessage("Kategorija cveta je obavezna.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Količina na stanju ne sme biti negativna.");
    }
}
