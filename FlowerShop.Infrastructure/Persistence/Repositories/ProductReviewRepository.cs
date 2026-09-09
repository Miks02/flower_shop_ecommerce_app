using FlowerShop.Domain.Entities.ProductReviews;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class ProductReviewRepository(AppDbContext context) : Repository<ProductReview>(context), IProductReviewRepository
{
    public async Task<ProductReview?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await Context.ProductReviews
            .AsSplitQuery()
            .Include(pr => pr.Product)
            .Include(pr => pr.User)
            .FirstOrDefaultAsync(pr => pr.Id == id, ct);
    }

    public async Task<ProductReview?> GetByProductAndReviewerAsync(int productId, string reviewerId, CancellationToken ct = default)
    {
        return await Context.ProductReviews
            .FirstOrDefaultAsync(pr => pr.ProductId == productId && pr.ReviewerId == reviewerId, ct);
    }
}
