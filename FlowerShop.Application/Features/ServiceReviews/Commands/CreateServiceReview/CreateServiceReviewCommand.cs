namespace FlowerShop.Application.Features.ServiceReviews.Commands.CreateServiceReview;

public record CreateServiceReviewCommand(string ReviewerId, int OrderId, int Rating, string? Comment);