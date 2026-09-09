namespace FlowerShop.Web.ViewModels.Components;

public record ChangePasswordViewModel
{
    public string CurrentPassword { get; init; } = null!;

    public string NewPassword { get; init; } = null!;

    public string ConfirmPassword { get; init; } = null!;
}