using FlowerShop.Web.Models;

namespace FlowerShop.Web.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAll();
    
    
}