using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.ProductReviews;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.ProductReviews.Commands.CreateProductReview;

public class CreateProductReviewHandler(
    IProductReviewRepository productReviewRepo,
    IProductRepository productRepo,
    INotificationService notificationService,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(CreateProductReviewCommand command, CancellationToken ct = default)
    {
        var product = await productRepo.GetByIdAsync(command.ProductId, ct);
        if (product is null || product.IsDeleted)
            return Result.Failure(ProductError.ProductNotFound(command.ProductId));

        var existingReview = await productReviewRepo.GetByProductAndReviewerAsync(command.ProductId, command.ReviewerId, ct);
        if (existingReview is not null)
            return Result.Failure(ProductReviewError.AlreadyReviewed(product.Name));

        var review = new ProductReview
        {
            ProductId = command.ProductId,
            ReviewerId = command.ReviewerId,
            Rating = command.Rating,
            Comment = command.Comment
        };

        productReviewRepo.Add(review);
        await unitOfWork.SaveAsync(ct);

        await notificationService.SendNotificationsToAllAdminsAsync(
            "Recenzija proizvoda",
            $"Korisnik je ostavio recenziju za proizvod {product.Name}.",
            NotificationType.Information,
            NotificationEntityType.ProductReview,
            product.Id);

        return Result.Success();
    }
}
