using FlowerShop.Models;

namespace FlowerShop.Services.Interfaces;

public interface ICategoryService
{
    public Task<IEnumerable<Category>> GetAll();
}