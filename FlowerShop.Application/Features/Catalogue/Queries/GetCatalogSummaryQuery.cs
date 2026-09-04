namespace FlowerShop.Application.Features.Catalogue.Queries;

public record GetCatalogSummaryQuery
{
    public int PriceRange { get; set; }
    public List<int> OccasionIds { get; set; } = [];
    public List<int> CategoryIds { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string Sort { get; set; } = "name_asc";
}
