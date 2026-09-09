namespace FlowerShop.Web.ViewModels.Components;

public record SettingsPageViewModel
{
    public ProfileSettingsViewModel ProfileVm { get; init; } = null!;

    public ChangePasswordViewModel ChangePasswordVm { get; init; } = null!;
}