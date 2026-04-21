using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;

namespace FlowerShop.Application.Features.Products.Queries.GetProductReferenceData;

public class GetProductReferenceDataHandler(
    IOccasionRepository occasionRepo, 
    ICategoryRepository categoryRepo) : IHandler
{
    public async Task<GetProductReferenceDataResponse> Handle(CancellationToken ct = default)
    {
        return new GetProductReferenceDataResponse
        {
            Occasions = await occasionRepo.GetAllAsync(ct),
            Categories = await categoryRepo.GetAllAsync(ct)
        };
    }
}