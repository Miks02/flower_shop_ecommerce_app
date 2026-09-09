namespace FlowerShop.Web.ViewModels.Components;

public record HeroViewModel
{
    public string Title { get; init; } = null!;
    public string? Subtitle { get; init; } = null!;
    public string BackgroundImage { get; init; } = null!;
    public bool IsHome { get; init; }
    public bool IsFullScreen { get; init; }
}