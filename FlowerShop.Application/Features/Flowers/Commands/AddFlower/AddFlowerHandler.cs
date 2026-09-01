using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Flowers.Commands.AddFlower;

public class AddFlowerHandler(
    IFlowerRepository flowerRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result<FlowerDto>> Handle(AddFlowerCommand command, CancellationToken ct = default)
    {
        if (await flowerRepo.ExistsAsync(command.Name, command.Color, command.FlowerCategory, ct))
            return Result<FlowerDto>.Failure(FlowerError.FlowerAlreadyExists(command.Name));

        var flower = new Flower
        {
            Name = command.Name,
            Color = command.Color,
            FlowerCategory = command.FlowerCategory,
            Stock = command.Stock
        };

        flowerRepo.Add(flower);
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
