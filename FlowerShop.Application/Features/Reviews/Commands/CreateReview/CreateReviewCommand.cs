namespace FlowerShop.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(string ReviewerId, int OrderId, int Rating, string? Comment);