using FlowerShop.Models;
using FlowerShop.Services.Interfaces;

namespace FlowerShop.Services.Mock;

public class MockCategoryService : ICategoryService
{
    private readonly List<Category> _categories;
    
    public MockCategoryService()
    {
        _categories = new List<Category>()
        {
            new Category { Id = 1, Name = "Buketi" },
            new Category { Id = 2, Name = "Aranžmani" },
            new Category {Id = 3, Name = "Specijalni pokloni"},
            new Category {Id = 4, Name = "101 Ruža"},
            new Category {Id = 5, Name = "Saksijsko cveće"},
            new Category {Id = 6, Name = "Box mede"},
            new Category {Id = 7, Name = "Dehidrirane ruže"},
            new Category {Id = 8, Name = "Venci i suze"}
        };
    }
    
    public Task<IEnumerable<Category>> GetAll()
    {
        return Task.FromResult(_categories.AsEnumerable());
    }
}