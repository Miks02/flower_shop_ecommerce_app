namespace FlowerShop.Domain.Entities.Flowers;

public record CatalogueFlowerDto
{
    public int Id { get; set; }

    public string FlowerName { get; set; } = null!;

    public string Color { get; set; } = null!;
}
