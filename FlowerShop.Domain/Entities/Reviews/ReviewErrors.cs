using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Reviews;

public static class ReviewError
{
    public static Error AlreadyReviewed(string orderNumber)
        => new Error("Review.AlreadyReviewed", $"Dostava {orderNumber} je već ocenjena.");
    
    public static Error NotReadyToReview(string orderNumber)
        => new Error("Review.NotReadyToReview", $"Dostava {orderNumber} će biti spremna za recenziju nakon završetka.");
    
    public static Error NotYourOrder(string orderNumber) 
        => new Error("Review.NotYourOrder", $"Dostava {orderNumber} ne pripada korisniku.");
    
}