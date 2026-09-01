using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Flowers.Commands.DeleteFlower;

public class DeleteFlowerHandler(
    IFlowerRepository flowerRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(DeleteFlowerCommand command, CancellationToken ct = default)
    {
        var flower = await flowerRepo.GetByIdAsync(command.Id, ct);
        if (flower is null)
            return Result.Failure(FlowerError.FlowerNotFound(command.Id.ToString()));

        if (await flowerRepo.IsUsedInProductsAsync(command.Id, ct))
            return Result.Failure(FlowerError.FlowerInUse(flower.Name));

        flowerRepo.Remove(flower);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
    }
}
