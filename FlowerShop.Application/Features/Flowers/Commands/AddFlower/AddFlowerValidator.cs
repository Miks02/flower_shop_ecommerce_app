using FluentValidation;

namespace FlowerShop.Application.Features.Flowers.Commands.AddFlower;

public class AddFlowerValidator : AbstractValidator<AddFlowerCommand>
{
    public AddFlowerValidator()
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
