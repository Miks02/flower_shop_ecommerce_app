using FluentValidation;

namespace FlowerShop.Application.Features.Users.Commands.UpdatePassword;

public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidator()
    {
        RuleFor(p => p.NewPassword)  
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati najmanje 6 karaktera.")
            .When(p => !string.IsNullOrEmpty(p.CurrentPassword)); 
        
        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("Potvrda lozinke je obavezna.")
            .Equal(p => p.NewPassword).WithMessage("Lozinke se ne poklapaju.")
            .When(p => !string.IsNullOrEmpty(p.CurrentPassword)); 
    }
}