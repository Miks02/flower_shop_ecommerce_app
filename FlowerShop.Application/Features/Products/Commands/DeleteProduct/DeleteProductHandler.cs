using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler(
    IProductRepository productRepo,
    IFlowerRepository flowerRepo,
    ICartRepository cartRepo,
    IFileService fileService,
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductHandler> logger) : IHandler
{
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken ct = default)
    {
        var product = await productRepo.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(ProductError.ProductNotFound(command.Id));

        var isReferencedElsewhere = product.ProductReviews.Count > 0
            || await cartRepo.ProductExistsInAnyCartAsync(product.Id, ct);

        if (isReferencedElsewhere)
        {
            logger.LogInformation(
                "Product {ProductId} is still referenced by reviews or carts; soft-deleting instead of removing it.",
                product.Id);

            product.IsDeleted = true;
            productRepo.Update(product);
            await unitOfWork.SaveAsync(ct);

            return Result.Success();
        }

        if (product.Stock > 0 && product.ProductFlowers.Count > 0)
        {
            var flowerIds = product.ProductFlowers.Select(pf => pf.FlowerId).Distinct().ToList();
            var flowers = await flowerRepo.GetFlowersByIdsAsync(flowerIds, ct);
            var flowersById = flowers.ToDictionary(f => f.Id);

            foreach (var pf in product.ProductFlowers)
            {
                if (flowersById.TryGetValue(pf.FlowerId, out var flower))
                {
                    flower.Stock += pf.Quantity * product.Stock;
                }
            }
        }

        var imageUrlToDelete = product.ImageUrl;

        productRepo.Remove(product);
        await unitOfWork.SaveAsync(ct);

        if (!string.IsNullOrEmpty(imageUrlToDelete))
        {
            await fileService.DeleteFile(imageUrlToDelete);
        }

        return Result.Success();
    }
}
