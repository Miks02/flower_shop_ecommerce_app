namespace FlowerShop.Application.Features.Flowers.Commands.UpdateFlowerStock;

public record UpdateFlowerStockCommand
{
    public int Id { get; init; }
    public int Quantity { get; init; }
}
