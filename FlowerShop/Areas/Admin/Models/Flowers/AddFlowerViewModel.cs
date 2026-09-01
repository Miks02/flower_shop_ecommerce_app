using FlowerShop.Domain.Enums;

namespace FlowerShop.Web.Areas.Admin.Models.Flowers;

public record AddFlowerViewModel
{
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public FlowerCategory FlowerCategory { get; init; }
    public int Stock { get; init; }
    public string SelectedFlowersJson { get; init; } = "[]";
}
