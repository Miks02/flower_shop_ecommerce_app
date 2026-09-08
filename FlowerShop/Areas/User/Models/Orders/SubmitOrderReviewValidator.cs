using FluentValidation;

namespace FlowerShop.Web.Areas.User.Models.Orders;

public class SubmitOrderReviewValidator : AbstractValidator<SubmitOrderReviewRequest>
{
    public SubmitOrderReviewValidator()
    {
        RuleFor(x => x.Rating)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Najmanja ocena je 1.")
            .LessThanOrEqualTo(5)
            .WithMessage("Ocena ne može biti veća od 5.");
    }
}