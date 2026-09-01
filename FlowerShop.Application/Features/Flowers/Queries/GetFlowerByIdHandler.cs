using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Flowers.Queries;

public class GetFlowerByIdHandler(IFlowerRepository flowerRepo) : IHandler
{
    public async Task<Result<FlowerDto>> Handle(int id, CancellationToken ct = default)
    {
        var flower = await flowerRepo.GetByIdAsync(id, ct);
        if (flower is null)
            return Result<FlowerDto>.Failure(FlowerError.FlowerNotFound(id.ToString()));

        var response = new FlowerDto
        {
            Id = flower.Id,
            Name = flower.Name,
            Color = flower.Color,
            FlowerCategory = flower.FlowerCategory,
            Stock = flower.Stock
        };

        return Result<FlowerDto>.Success(response);
    }
}
