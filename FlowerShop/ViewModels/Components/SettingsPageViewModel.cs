namespace FlowerShop.ViewModels.Components;

public class SettingsPageViewModel
{
    public ProfileSettingsViewModel ProfileVm { get; set; } = null!;
    
    public ChangePasswordViewModel ChangePasswordVm { get; set; } = new ChangePasswordViewModel();

}