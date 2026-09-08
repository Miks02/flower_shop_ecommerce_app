namespace FlowerShop.Domain.Entities.ProductReviews;

public interface IProductReviewRepository
{
    void Add(ProductReview productReview);
    void Update(ProductReview productReview);
    void Remove(ProductReview productReview);
    Task<ProductReview?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductReview?> GetByProductAndReviewerAsync(int productId, string reviewerId, CancellationToken ct = default);
}