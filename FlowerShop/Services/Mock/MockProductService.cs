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
        };

        _products = new List<Product>()
        {
            new Product { Id = 1, Name = "Ljubavno Crveno", Price = 5999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Buket crvenih ruza.jpg", Category = _categories[0], Occasions = _occasions },
            new Product { Id = 2, Name = "Nebeska Magija", Price = 9999, Stock = 100, Description = "Buket za svadbe i rodjenje deteta", ImageUrl = "./AppImages/Proizvodi/Buketi/Buket Sky.jpg", Category = _categories[0], Occasions = _occasions },
            new Product { Id = 3, Name = "Tropska Romansa", Price = 6999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Buket Tropska Romansa.jpg", Category = _categories[0], Occasions = _occasions },
            new Product { Id = 4, Name = "Osmeh Suncokreta", Price = 999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Buket Osmeh Suncokreta.jpg", Category = _categories[0], Occasions = _occasions },
            new Product { Id = 5, Name = "Romantični Raj", Price = 999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Buket Romantični Raj.jpg", Category = _categories[0], Occasions = _occasions },
            new Product { Id = 6, Name = "Crveno Carstvo", Price = 1999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Crveno Carstvo.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 7, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 8, Name = "Buket crvenih ruža", Price = 999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Orhideje i Mini Ruže.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 9, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 10, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 11, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 12, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 13, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 14, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 15, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            new Product { Id = 16, Name = "Ruže i Lizijantus", Price = 4999, Stock = 100, Description = "Buket za sve prilike", ImageUrl = "./AppImages/Proizvodi/Buketi/Ruže i Lizijantus.jpg", Category = _categories[1], Occasions = _occasions },
            

        };
    }
    
    public Task<IEnumerable<Product>> GetAll()
    {
        return Task.FromResult(_products.AsEnumerable());
        
    }


}