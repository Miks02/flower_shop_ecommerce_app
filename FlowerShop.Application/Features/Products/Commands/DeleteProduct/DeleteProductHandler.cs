using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler(
    IProductRepository productRepo,
    IFlowerRepository flowerRepo,
    IFileService fileService,
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductHandler> logger) : IHandler
{
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken ct = default)
    {
        string? imageUrlToDelete = null;

        try
        {
            await unitOfWork.BeginTransactionAsync(ct);

            var product = await productRepo.GetByIdAsync(command.Id, ct);
            if (product is null)
                return Result.Failure(ProductError.ProductNotFound(command.Id));

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

            imageUrlToDelete = product.ImageUrl;

            productRepo.Remove(product);
            await unitOfWork.SaveAsync(ct);
            await unitOfWork.CommitAsync(ct);

            if (!string.IsNullOrEmpty(imageUrlToDelete))
            {
                await fileService.DeleteFile(imageUrlToDelete);
            }
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError(ex, "An exception occurred while deleting product {ProductId}.", command.Id);
            throw;
        }

        return Result.Success();
    }
}
