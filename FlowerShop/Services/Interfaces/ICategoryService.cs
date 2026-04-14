using FlowerShop.Web.Models;

namespace FlowerShop.Web.Services.Interfaces;

public interface ICategoryService
{
    public Task<IEnumerable<Category>> GetAll();
}