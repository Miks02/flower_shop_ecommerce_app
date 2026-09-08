using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.ServiceReviews;

public static class ServiceReviewError
{
    public static Error AlreadyReviewed(string orderNumber)
        => new Error("ServiceReview.AlreadyReviewed", $"Dostava {orderNumber} je već ocenjena.");

    public static Error NotReadyToReview(string orderNumber)
        => new Error("ServiceReview.NotReadyToReview", $"Dostava {orderNumber} će biti spremna za recenziju nakon završetka.");

    public static Error NotYourOrder(string orderNumber)
        => new Error("ServiceReview.NotYourOrder", $"Dostava {orderNumber} ne pripada korisniku.");

}