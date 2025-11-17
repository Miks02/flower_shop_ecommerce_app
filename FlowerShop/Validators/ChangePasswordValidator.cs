using FlowerShop.ViewModels.Components;
using FluentValidation;

namespace FlowerShop.Validators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordViewModel>
{
    public ChangePasswordValidator()
    {
        
        RuleFor(p => p.NewPassword)  
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati najmanje 8 karaktera.")
            .Matches(@"[A-Z]+").WithMessage("Lozinka mora sadržati barem jedno veliko slovo.")
            .Matches(@"[a-z]+").WithMessage("Lozinka mora sadržati barem jedno malo slovo.")
            .Matches(@"[0-9]+").WithMessage("Lozinka mora sadržati barem jedan broj.")
            .When(p => !string.IsNullOrEmpty(p.CurrentPassword)); 
        
        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("Potvrda lozinke je obavezna.")
            .Equal(p => p.NewPassword).WithMessage("Lozinke se ne poklapaju.")
            .When(p => !string.IsNullOrEmpty(p.CurrentPassword)); 
            
    }
}