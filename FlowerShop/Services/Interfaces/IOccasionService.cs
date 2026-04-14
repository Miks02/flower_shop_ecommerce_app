using FlowerShop.Web.Models;

namespace FlowerShop.Web.Services.Interfaces;

public interface IOccasionService
{
    public Task<IEnumerable<Occasion>> GetAll();
}