using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Domain.Entities.ServiceReviews;

public class ServiceReview
{
    public int Id { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string ReviewerId { get; set; } = null!;
    public User Reviewer { get; set; } = null!;
}