namespace FlowerShop.Web.ViewModels.Components;

public record LoginViewModel
{
    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;

    public bool RememberMe { get; init; }
}