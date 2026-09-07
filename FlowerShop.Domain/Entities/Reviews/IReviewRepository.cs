namespace FlowerShop.Domain.Entities.Reviews;

public interface IReviewRepository
{
    void Add(Review review);
    void Update(Review review);
    void Remove(Review review);
}