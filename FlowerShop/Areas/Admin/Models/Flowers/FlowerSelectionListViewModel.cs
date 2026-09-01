using FlowerShop.Domain.Entities.Flowers;

namespace FlowerShop.Web.Areas.Admin.Models.Flowers;

public record FlowerSelectionListViewModel
{
    public IReadOnlyList<FlowerDto> AvailableFlowers { get; init; } = [];
    public List<SelectedFlowerDto> SelectedFlowers { get; init; } = [];
}
