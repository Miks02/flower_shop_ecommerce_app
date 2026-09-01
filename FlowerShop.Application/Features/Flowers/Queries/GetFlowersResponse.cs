using FlowerShop.Domain.Entities.Flowers;

namespace FlowerShop.Application.Features.Flowers.Queries;

public record GetFlowersResponse
{
    public IReadOnlyList<FlowerDto> Flowers { get; init; }
}