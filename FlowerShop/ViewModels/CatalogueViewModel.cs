using FlowerShop.Web.Models;

namespace FlowerShop.Web.ViewModels;

public class CatalogueViewModel
{
    public List<Product> Products { get; set; } = new List<Product>();
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<Occasion> Occasions { get; set; } = new List<Occasion>();
}