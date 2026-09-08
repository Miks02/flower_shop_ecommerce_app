using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.ProductReviews;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.ProductReviews.Commands.DeleteProductReview;

public class DeleteProductReviewHandler(
    IProductReviewRepository productReviewRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(DeleteProductReviewCommand command, CancellationToken ct = default)
    {
        var review = await productReviewRepo.GetByIdAsync(command.ProductReviewId, ct);
        if (review is null)
            return Result.Failure(ProductReviewError.NotFound(command.ProductReviewId));

        if (review.ReviewerId != command.RequesterId)
            return Result.Failure(ProductReviewError.NotYourReview());

        productReviewRepo.Remove(review);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
    }
}
