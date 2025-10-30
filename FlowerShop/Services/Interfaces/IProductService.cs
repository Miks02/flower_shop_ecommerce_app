using FlowerShop.Models;

namespace FlowerShop.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAll();
    
    
}