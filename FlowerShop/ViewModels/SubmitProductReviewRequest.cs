namespace FlowerShop.Web.ViewModels;

public record SubmitProductReviewRequest
{
    public int Rating { get; init; }
    public string? Comment { get; init; }
}
