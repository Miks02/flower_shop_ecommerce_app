using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;

namespace FlowerShop.Domain.Entities.Reviews;

public class Review
{
    public int Id { get; set; }
    public decimal Rating { get; set; }
    public string? Comment { get; set; }

    public string DelivererId { get; set; } = null!;
    public Deliverer Deliverer { get; set; } = null!;

    public string ReviewerId { get; set; } = null!;
    public User Reviewer { get; set; } = null!;
}