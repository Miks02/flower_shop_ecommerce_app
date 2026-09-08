using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.ServiceReviews;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.ServiceReviews.Commands.CreateServiceReview;

public class CreateServiceReviewHandler(
    IServiceReviewRepository serviceReviewRepo,
    IOrderRepository orderRepo,
    INotificationService notificationService,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(CreateServiceReviewCommand command, CancellationToken ct = default)
    {
        var orderToReview = await orderRepo.GetByIdAsync(command.OrderId, ct);

        if(orderToReview is null)
            return Result.Failure(OrderError.OrderNotFound(command.OrderId.ToString()));

        if (orderToReview.UserId != command.ReviewerId)
            return Result.Failure(ServiceReviewError.NotYourOrder(orderToReview.OrderNumber));

        if (orderToReview.DeliveryStatus != DeliveryStatus.Delivered)
            return Result.Failure(ServiceReviewError.NotReadyToReview(orderToReview.OrderNumber));

        if(orderToReview.ServiceReviewId is not null)
            return Result.Failure(ServiceReviewError.AlreadyReviewed(orderToReview.OrderNumber));

        var review = new ServiceReview
        {
            ReviewerId = command.ReviewerId,
            OrderId = command.OrderId,
            Rating = command.Rating,
            Comment = command.Comment
        };

        serviceReviewRepo.Add(review);
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