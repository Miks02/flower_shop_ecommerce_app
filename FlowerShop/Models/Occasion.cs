using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models;

public class Occasion
{
    public int Id { get; set; }
    [StringLength(50)]
    public string Name { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}