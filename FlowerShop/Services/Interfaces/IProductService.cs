
using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Web.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAll();
    
    
}