using System.ComponentModel.DataAnnotations;
using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Domain.Entities.Categories;

public class Category
{
    public int Id { get; set; }
    
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}