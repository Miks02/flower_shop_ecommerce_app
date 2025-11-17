using FlowerShop.ViewModels.Components;
using FluentValidation;

namespace FlowerShop.Validators;

public class SettingsPageValidator : AbstractValidator<SettingsPageViewModel>
{

    public SettingsPageValidator()
    {
        RuleFor(p => p.ProfileVm)
            .SetValidator(new ProfileSettingsValidator());

        RuleFor(p => p.ChangePasswordVm)
            .SetValidator(new ChangePasswordValidator());
    }
    
}