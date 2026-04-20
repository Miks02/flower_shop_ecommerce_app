namespace FlowerShop.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery
{
    public string? SearchBy { get; set; }
    public string? SortBy { get; set; }
    public int? CategoryId { get; set; }
    public bool IsDeleted { get; set; }
    public IReadOnlyList<int> OccasionIds { get; set; } = [];
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}