using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Flowers.Commands.UpdateFlowerStock;

public class UpdateFlowerStockHandler(
    IFlowerRepository flowerRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result<FlowerDto>> Handle(UpdateFlowerStockCommand command, CancellationToken ct = default)
    {
        var flower = await flowerRepo.GetByIdAsync(command.Id, ct);
        if (flower is null)
            return Result<FlowerDto>.Failure(FlowerError.FlowerNotFound(command.Id.ToString()));

        flower.Stock += command.Quantity;

        flowerRepo.Update(flower);
        await unitOfWork.SaveAsync(ct);

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
