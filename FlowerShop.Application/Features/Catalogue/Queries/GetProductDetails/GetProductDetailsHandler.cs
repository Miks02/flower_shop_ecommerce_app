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

        var reviews = product.ProductReviews
            .OrderByDescending(pr => pr.CreatedAt)
            .Select(pr => new ProductReviewDto
            {
                Id = pr.Id,
                ReviewerName = $"{pr.User.FirstName} {pr.User.LastName}".Trim(),
                Rating = pr.Rating,
                Comment = pr.Comment,
                CreatedAt = pr.CreatedAt
            })
            .ToList();

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
                .ToList(),
            AverageRating = product.ProductReviews.Count > 0 ? product.ProductReviews.Average(pr => pr.Rating) : null,
            ReviewCount = product.ProductReviews.Count,
            CurrentUserReviewId = request.CurrentUserId is null
                ? null
                : product.ProductReviews.FirstOrDefault(pr => pr.ReviewerId == request.CurrentUserId)?.Id,
            Reviews = reviews
        };

        return Result<GetProductDetailsResponse>.Success(response);
    }
}
