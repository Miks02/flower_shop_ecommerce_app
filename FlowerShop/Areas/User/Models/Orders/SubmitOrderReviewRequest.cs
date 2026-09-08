namespace FlowerShop.Web.Areas.User.Models.Orders;

public class SubmitOrderReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}