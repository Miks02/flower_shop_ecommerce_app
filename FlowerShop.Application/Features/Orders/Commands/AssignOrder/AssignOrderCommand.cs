namespace FlowerShop.Application.Features.Orders.Commands.AssignOrder;

public record AssignOrderCommand
{
    public int OrderId { get; init; }
    public string DelivererId { get; init; } = null!;
    
};