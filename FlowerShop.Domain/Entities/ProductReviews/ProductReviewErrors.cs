using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.ProductReviews;

public static class ProductReviewError
{
    public static Error AlreadyReviewed(string productName)
        => new Error("ProductReview.AlreadyReviewed", $"Već ste ostavili recenziju za proizvod '{productName}'.");

    public static Error NotFound(int? identifier = null)
    {
        string message = identifier is null
                    ? "Recenzija nije pronađena."
                    : $"Recenzija sa identifikatorom '{identifier}' nije pronađena.";

        return new Error("ProductReview.NotFound", message);
    }

    public static Error NotYourReview()
        => new Error("ProductReview.NotYourReview", "Nije moguće obrisati tudju recenziju.");
}
