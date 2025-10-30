using FlowerShop.Models;

namespace FlowerShop.Services.Interfaces;

public interface IOccasionService
{
    public Task<IEnumerable<Occasion>> GetAll();
}