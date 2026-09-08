namespace FlowerShop.Application.Features.ProductReviews.Commands.CreateProductReview;

public record CreateProductReviewCommand(string ReviewerId, int ProductId, int Rating, string? Comment);
