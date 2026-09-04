using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Domain.Entities.Carts;

public class CartItem 
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;
}