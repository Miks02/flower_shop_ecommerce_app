namespace FlowerShop.Web.Areas.Admin.Models.Flowers;

public record UpdateFlowerStockViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CurrentStock { get; init; }
    public int Quantity { get; init; } = 1;
    public string SelectedFlowersJson { get; init; } = "[]";
}
