using FluentValidation;

namespace FlowerShop.Web.ViewModels;

public class SubmitProductReviewValidator : AbstractValidator<SubmitProductReviewRequest>
{
    public SubmitProductReviewValidator()
    {
        RuleFor(x => x.Rating)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Najmanja ocena je 1.")
            .LessThanOrEqualTo(5)
            .WithMessage("Ocena ne može biti veća od 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Komentar ne može biti duži od 1000 karaktera.");
    }
}
