using FluentValidation;

namespace FlowerShop.Web.Areas.Admin.Models.Flowers;

public class UpdateFlowerStockViewModelValidator : AbstractValidator<UpdateFlowerStockViewModel>
{
    public UpdateFlowerStockViewModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id cveta je obavezan.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Količina za dodavanje na zalihu mora biti pozitivna.");
    }
}
