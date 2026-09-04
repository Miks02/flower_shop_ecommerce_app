using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Catalogue.Queries.GetProductDetails;

public class GetProductDetailsHandler(IProductRepository productRepo) : IHandler
{
    public async Task<Result<GetProductDetailsResponse>> Handle(
        GetProductDetailsQuery request,
        CancellationToken ct = default)
    {
        var product = await productRepo.GetByIdAsync(request.Id, ct);
        if (product is null || product.IsDeleted)
            return Result<GetProductDetailsResponse>.Failure(ProductError.ProductNotFound(request.Id));

        var isOnPromotion = product.PromoPrice is > 0;

        var response = new GetProductDetailsResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            PromoPrice = isOnPromotion ? product.PromoPrice : null,
            IsOnPromotion = isOnPromotion,
            DiscountType = product.DiscountType,
            Stock = product.Stock,
            CategoryName = product.Category.Name,
            Occasions = product.Occasions.Select(o => o.Name).ToList(),
            Composition = product.ProductFlowers
                .Select(pf => new FlowerCompositionDto
                {
                    Name = pf.Flower.Name,
                    Color = pf.Flower.Color,
                    Quantity = pf.Quantity,
                    Category = pf.Flower.FlowerCategory
                })
                .OrderBy(f => f.Name)
                .ToList()
        };

        return Result<GetProductDetailsResponse>.Success(response);
    }
}
