using FlowerShop.Models;
using FlowerShop.Services.Interfaces;

namespace FlowerShop.Services.Mock;

public class MockOccasionService : IOccasionService
{
    private readonly List<Occasion> _occasions;

    public MockOccasionService()
    {
        _occasions = new List<Occasion>()
        {
            new Occasion { Id = 1, Name = "Dan zaljubljenih" },
            new Occasion { Id = 2, Name = "8. Mart - Dan žena" },
            new Occasion { Id = 3, Name = "Rodjendan" },
            new Occasion { Id = 4, Name = "Svadba i venčanje" },
            new Occasion { Id = 5, Name = "Sahrane i pomeni" }
        };
    }
    public Task<IEnumerable<Occasion>> GetAll()
    {
        return Task.FromResult(_occasions.AsEnumerable());
    }
}