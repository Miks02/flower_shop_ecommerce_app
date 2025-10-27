using System.ComponentModel.DataAnnotations;
using FlowerShop.Enums;

namespace FlowerShop.Models;

public class FlowerType
{
    public int Id { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int Stock { get; set; }
    
    [StringLength(50)]
    public string Color {get; set; } = null!;
    
    public FlowerCategory FlowerCategory { get; set; } 
    public ICollection<ProductFlower> ProductFlowers { get; set; } = new List<ProductFlower>();
}