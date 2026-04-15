using FlowerShop.Domain.Entities.Categories;

namespace FlowerShop.Web.Services.Interfaces;

public interface ICategoryService
{
    public Task<IEnumerable<Category>> GetAll();
}