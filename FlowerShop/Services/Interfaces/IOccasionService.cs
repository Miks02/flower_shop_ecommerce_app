using FlowerShop.Domain.Entities.Ocassions;

namespace FlowerShop.Web.Services.Interfaces;

public interface IOccasionService
{
    public Task<IEnumerable<Occasion>> GetAll();
}