using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler(
    IProductRepository productRepo,
    IFlowerRepository flowerRepo,
    ICategoryRepository categoryRepo,
    IOccasionRepository occasionRepo,
    IFileService fileService,
    IUnitOfWork unitOfWork,
    ILogger<UpdateProductHandler> logger) : IHandler
{
    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken ct = default)
    {
        var uploadedFilePath = "";

        try
        {
            await unitOfWork.BeginTransactionAsync(ct);

            var product = await productRepo.GetByIdAsync(command.Id, ct);
            if (product is null)
                return Result.Failure(ProductError.ProductNotFound(command.Id));

            if (command.Name != product.Name && await productRepo.ExistsByNameAsync(command.Name, ct))
            {
                return Result.Failure(ProductError.ProductAlreadyExists());
            }

            if (!await categoryRepo.ExistsAsync(command.CategoryId, ct))
                return Result.Failure(CategoryError.CategoryNotFound(command.CategoryId.ToString()));

            var flowerOperationResult = await ValidateAndUpdateFlowersStockAsync(product, command.Flowers, command.Stock, ct);
            if (!flowerOperationResult.IsSuccess)
                return flowerOperationResult;

            var occasions = await occasionRepo.GetOccasionsByIdsAsync(command.Occasions, ct);
            var occasionIds = occasions.Select(o => o.Id).ToList();
            var invalidOccasionIds = GetInvalidOccasionIds(command.Occasions, occasionIds);

            if (invalidOccasionIds.Any())
                return Result.Failure(OccasionError.OccasionsNotFound(invalidOccasionIds));

            string? oldImageUrlToDelete = null;

            if (command.ProductImage != null && command.ProductImage.Length > 0)
            {
                var imagePath = await fileService.UploadFile(command.ProductImage, "", "product-images");
                if (!imagePath.IsSuccess)
                    return Result.Failure(imagePath.Errors.ToArray());

                uploadedFilePath = imagePath.Payload!;
                oldImageUrlToDelete = product.ImageUrl;
                product.ImageUrl = uploadedFilePath;
            }

            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.Stock = command.Stock;
            product.CategoryId = command.CategoryId;
            product.Occasions = occasions.ToList();

            product.ProductFlowers.Clear();
            foreach (var flower in command.Flowers)
            {
                product.ProductFlowers.Add(new ProductFlower
                {
                    ProductId = product.Id,
                    FlowerId = flower.Id,
                    Quantity = flower.Quantity
                });
            }

            productRepo.Update(product);
            await unitOfWork.SaveAsync(ct);
            await unitOfWork.CommitAsync(ct);

            if (!string.IsNullOrEmpty(oldImageUrlToDelete))
            {
                await fileService.DeleteFile(oldImageUrlToDelete);
            }
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError(ex, "An exception has occurred during transaction.");

            if (!string.IsNullOrEmpty(uploadedFilePath))
                await fileService.DeleteFile(uploadedFilePath);
            throw;
        }

        return Result.Success();
    }

    private async Task<Result> ValidateAndUpdateFlowersStockAsync(
        Product product,
        IReadOnlyList<FlowerItemDto> inputFlowers,
        int newProductStock,
        CancellationToken ct = default)
    {
        var inputFlowerIds = inputFlowers.Select(f => f.Id).ToList();
        var existingFlowerIds = product.ProductFlowers.Select(pf => pf.FlowerId).ToList();
        var allFlowerIds = inputFlowerIds.Union(existingFlowerIds).Distinct().ToList();

        var flowers = await flowerRepo.GetFlowersByIdsAsync(allFlowerIds, ct);

        var invalidFlowers = GetInvalidFlowerIds(inputFlowerIds, flowers.Select(f => f.Id).ToList());
        if (invalidFlowers.Any())
            return Result.Failure(FlowerError.FlowersNotFound(invalidFlowers));

        var oldUsageByFlowerId = product.ProductFlowers
            .GroupBy(pf => pf.FlowerId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity) * product.Stock);

        var newUsageByFlowerId = inputFlowers
            .GroupBy(f => f.Id)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity) * Math.Max(0, newProductStock));

        var insufficientStockFlowersIds = new List<int>();

        foreach (var flower in flowers)
        {
            var oldUsage = oldUsageByFlowerId.GetValueOrDefault(flower.Id, 0);
            var newUsage = newUsageByFlowerId.GetValueOrDefault(flower.Id, 0);
            var netChange = newUsage - oldUsage;

            if (netChange > 0 && flower.Stock < netChange)
            {
                insufficientStockFlowersIds.Add(flower.Id);
            }
        }

        if (insufficientStockFlowersIds.Count > 0)
            return Result.Failure(FlowerError.InsufficientStock(insufficientStockFlowersIds));

        foreach (var flower in flowers)
        {
            var oldUsage = oldUsageByFlowerId.GetValueOrDefault(flower.Id, 0);
            var newUsage = newUsageByFlowerId.GetValueOrDefault(flower.Id, 0);
            var netChange = newUsage - oldUsage;

            flower.Stock -= netChange;
        }

        return Result.Success();
    }

    private IReadOnlyList<int> GetInvalidOccasionIds(IReadOnlyList<int> inputIds, IReadOnlyList<int> occasionIds)
    {
        return inputIds.Except(occasionIds).ToList();
    }

    private IReadOnlyList<int> GetInvalidFlowerIds(IReadOnlyList<int> inputIds, IReadOnlyList<int> flowerIds)
    {
        return inputIds.Except(flowerIds).ToList();
    }
}