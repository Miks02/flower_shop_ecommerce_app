using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductHandler(
    IProductRepository productRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(ArchiveProductCommand command, CancellationToken ct = default)
    {
        var product = await productRepo.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(ProductError.ProductNotFound(command.Id));

        if (product.IsDeleted)
            product.IsDeleted = false;
        else
            product.IsDeleted = true;

        productRepo.Update(product);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
    }
}
