using FlowerShop.Domain.Entities.ServiceReviews;
using FlowerShop.Infrastructure.Persistence.EntityFramework;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class ServiceReviewRepository(AppDbContext context) : Repository<ServiceReview>(context), IServiceReviewRepository
{

}