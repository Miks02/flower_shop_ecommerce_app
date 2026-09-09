namespace FlowerShop.Web.Areas.User.Models.Orders;

public record SubmitOrderReviewRequest
{
    public int Rating { get; init; }
    public string? Comment { get; init; }
}