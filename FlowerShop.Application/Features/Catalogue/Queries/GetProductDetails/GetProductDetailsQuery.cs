namespace FlowerShop.Application.Features.Catalogue.Queries.GetProductDetails;

public record GetProductDetailsQuery
{
    public int Id { get; init; }
    public string? CurrentUserId { get; init; }
}
