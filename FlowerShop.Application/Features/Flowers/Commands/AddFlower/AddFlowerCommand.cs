using FlowerShop.Domain.Enums;

namespace FlowerShop.Application.Features.Flowers.Commands.AddFlower;

public record AddFlowerCommand
{
    public string Name { get; init; } = null!;
    public string Color { get; init; } = null!;
    public FlowerCategory FlowerCategory { get; init; }
    public int Stock { get; init; }
}
