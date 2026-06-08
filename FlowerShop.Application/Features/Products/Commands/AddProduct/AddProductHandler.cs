using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Products.Commands.AddProduct;

public class AddProductHandler (
    IProductRepository productRepo,
    IFlowerRepository flowerRepo,
    ICategoryRepository categoryRepo, 
    IOccasionRepository occasionRepo,
    IFileService fileService,
    IUnitOfWork unitOfWork,
    ILogger<AddProductHandler> logger) : IHandler
{
    
    public async Task<Result> Handle(AddProductCommand command, CancellationToken ct = default)
    {
        var uploadedFilePath = "";

        try
        {
            await unitOfWork.BeginTransactionAsync(ct);
            
            if(!await categoryRepo.ExistsAsync(command.CategoryId, ct))
                return Result.Failure(CategoryError.CategoryNotFound(command.CategoryId.ToString()));

            var flowerOperationResult = await ValidateAndUpdateFlowersStockAsync(command.Flowers, command.Stock, ct);
            if (!flowerOperationResult.IsSuccess)
                return flowerOperationResult;
        
            var occasions = await occasionRepo.GetOccasionsByIdsAsync(command.Occasions, ct);
        
            var occasionIds = occasions
                .Select(o => o.Id)
                .ToList();
        
            var invalidOccasionIds = GetInvalidOccasionIds(command.Occasions, occasionIds);

            if (invalidOccasionIds.Any())
                return Result.Failure(OccasionError.OccasionsNotFound(invalidOccasionIds)); 
        
            var imagePath = await fileService.UploadFile(command.ProductImage, "", "product-images");

            if (!imagePath.IsSuccess)
                return Result.Failure(imagePath.Errors.ToArray());
        
            uploadedFilePath = imagePath.Payload!;

            var newProduct = new Product
            {
                CreatedBy = command.UserId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.Stock,
                CategoryId = command.CategoryId,
                ProductFlowers = command.Flowers.Select(f => new ProductFlower
                {
                    FlowerId = f.Id,
                    Quantity = f.Quantity
                }).ToList(),
                ImageUrl = uploadedFilePath,
                Occasions = occasions.ToList()
            };
        
            productRepo.Add(newProduct);
            await unitOfWork.SaveAsync(ct);
            await unitOfWork.CommitAsync(ct);
            
        }
        catch(Exception ex) {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError(ex, "An exception has occurred during transaction.");

            if (!string.IsNullOrEmpty(uploadedFilePath))
                await fileService.DeleteFile(uploadedFilePath);
            throw;
        }
       
        
        return Result.Success();
    }
    
    private async Task<Result> ValidateAndUpdateFlowersStockAsync(IReadOnlyList<FlowerItemDto> inputFlowers, int productStock = 1, CancellationToken ct = default)
    {
        var flowerIds = inputFlowers.Select(f => f.Id).ToList();
        
        var flowers = await flowerRepo.GetFlowersByIdsAsync(flowerIds, ct);

        var invalidFlowers = GetInvalidFlowerIds(flowerIds, flowers.Select(f => f.Id).ToList());
        if(invalidFlowers.Any())
            return Result.Failure(FlowerError.FlowersNotFound(invalidFlowers));

        if (productStock <= 0)
            return Result.Success();

        var insufficientStockFlowersIds = new List<int>();

        foreach (var flower in inputFlowers)
        {
            var checkedFlower = flowers.First(f => f.Id == flower.Id);
            if (checkedFlower.Stock < flower.Quantity * productStock)
                insufficientStockFlowersIds.Add(flower.Id);
        }

        if (insufficientStockFlowersIds.Count > 0)
            return Result.Failure(FlowerError.InsufficientStock(insufficientStockFlowersIds));

        foreach (var flower in inputFlowers)
        {
            var checkedFlower = flowers.First(f => f.Id == flower.Id);
            checkedFlower.Stock -= flower.Quantity * productStock;
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