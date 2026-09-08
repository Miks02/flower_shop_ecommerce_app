using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Reviews;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewHandler(
    IReviewRepository reviewRepo, 
    IOrderRepository orderRepo,
    INotificationService notificationService,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(CreateReviewCommand command, CancellationToken ct = default)
    {
        var orderToReview = await orderRepo.GetByIdAsync(command.OrderId, ct);
        
        if(orderToReview is null)
            return Result.Failure(OrderError.OrderNotFound(command.OrderId.ToString()));

        if (orderToReview.UserId != command.ReviewerId)
            return Result.Failure(ReviewError.NotYourOrder(orderToReview.OrderNumber));

        if (orderToReview.DeliveryStatus != DeliveryStatus.Delivered)
            return Result.Failure(ReviewError.NotReadyToReview(orderToReview.OrderNumber));
        
        if(orderToReview.ReviewId is not null)
            return Result.Failure(ReviewError.AlreadyReviewed(orderToReview.OrderNumber));
        
        var review = new Review
        {
            ReviewerId = command.ReviewerId,
            OrderId = command.OrderId,
            Rating = command.Rating,
            Comment = command.Comment
        };

        reviewRepo.Add(review);
        await unitOfWork.SaveAsync(ct);

        await notificationService.SendNotificationAsync(
            orderToReview.DelivererId!, 
            "Recenzija", $"Korisnik je ostavio recenziju za porudžbinu {orderToReview.OrderNumber}.",
            NotificationType.Information,
            NotificationEntityType.Order,
            orderToReview.Id);
        return Result.Success();
    }
}