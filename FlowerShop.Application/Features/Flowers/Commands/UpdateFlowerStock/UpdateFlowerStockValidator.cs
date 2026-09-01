using FluentValidation;

namespace FlowerShop.Application.Features.Flowers.Commands.UpdateFlowerStock;

public class UpdateFlowerStockValidator : AbstractValidator<UpdateFlowerStockCommand>
{
    public UpdateFlowerStockValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id cveta je obavezan.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Količina za dodavanje na zalihu mora biti pozitivna.");
    }
}
