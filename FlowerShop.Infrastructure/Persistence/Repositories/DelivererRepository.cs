using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Infrastructure.Persistence.EntityFramework;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class DelivererRepository(AppDbContext context) : Repository<Deliverer>(context), IDelivererRepository
{
    
}