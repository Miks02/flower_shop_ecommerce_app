namespace FlowerShop.Domain.Entities.Products;

public interface IProductRepository
{
    void Add(Product product);
    void Update(Product product);
    void Remove(Product product);
}