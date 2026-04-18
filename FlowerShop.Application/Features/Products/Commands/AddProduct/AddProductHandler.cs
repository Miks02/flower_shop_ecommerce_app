using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Products.Commands.AddProduct;

public class AddProductHandler (
    IProductRepository productRepo,
    IFlowerRepository flowerRepo,
    ICategoryRepository categoryRepo, 
    IOccasionRepository occasionRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    
    public async Task<Result> Handle(AddProductCommand command, CancellationToken ct = default)
    {
        
        if(!await categoryRepo.ExistsAsync(command.CategoryId, ct))
            return Result.Failure(CategoryError.CategoryNotFound(command.CategoryId.ToString()));
        
        var flowerIds = command.Flowers.Select(f => f.id).ToList();
        
        var invalidFlowerIds = await flowerRepo.GetInvalidFlowerIdsAsync(flowerIds, ct);
        
        if(invalidFlowerIds.Any())
            return Result.Failure(FlowerError.FlowersNotFound(invalidFlowerIds));
        
        var occasions = await occasionRepo.GetOccasionsByIdsAsync(command.Occasions, ct);
        
        var occasionIds = occasions
            .Select(o => o.Id)
            .ToList();
        
        var invalidOccasionIds = GetInvalidOccasionIds(command.Occasions, occasionIds);

        if (invalidOccasionIds.Any())
            return Result.Failure(OccasionError.OccasionsNotFound(invalidOccasionIds));
        
        
        var newProduct = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Stock = command.Stock,
            CategoryId = command.CategoryId,
            ProductFlowers = command.Flowers.Select(f => new ProductFlower
            {
                FlowerId = f.id,
                Quantity = f.quantity
            }).ToList(),
            Occasions = occasions.ToList()
        };
        
        productRepo.Add(newProduct);
        await unitOfWork.SaveAsync(ct);
        
        return Result.Success();
    }
    
    private IReadOnlyList<int> GetInvalidOccasionIds(IReadOnlyList<int> inputIds, IReadOnlyList<int> occasionIds)
    {
        return inputIds.Except(occasionIds).ToList();
    }
}