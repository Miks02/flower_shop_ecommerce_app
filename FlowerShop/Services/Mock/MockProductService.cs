using FlowerShop.Models;
using FlowerShop.Services.Interfaces;

namespace FlowerShop.Services.Mock;

public class MockProductService : IProductService
{
    private readonly List<Category> _categories;
    private readonly List<Product> _products;
    private readonly List<Occasion> _occasions;
    
    public MockProductService()
    {
        _categories = new List<Category>()
        {
            new Category { Id = 1, Name = "Buketi" },
            new Category { Id = 2, Name = "Aranžmani" }
        };

        _occasions = new List<Occasion>()
        {
            new Occasion { Id = 1, Name = "Dan zaljubljenih" },
            new Occasion { Id = 2, Name = "8. Mart - Dan žena" },
            new Occasion { Id = 3, Name = "Rodjendan" },
            new Occasion { Id = 4, Name = "Svadba i venčanje" }
        };

        _products = new List<Product>()
        {
            new Product { Id = 1, Name = "Buket crvenih ruža", Price = 999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/buket.jpg", Category = _categories[0], Occasions = _occasions }

        };
    }
    
    public Task<IEnumerable<Product>> GetAll()
    {
        return Task.FromResult(_products.AsEnumerable());
        
    }
}