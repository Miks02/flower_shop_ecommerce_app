namespace FlowerShop.Domain.Entities.ServiceReviews;

public interface IServiceReviewRepository
{
    void Add(ServiceReview serviceReview);
    void Update(ServiceReview serviceReview);
    void Remove(ServiceReview serviceReview);
}