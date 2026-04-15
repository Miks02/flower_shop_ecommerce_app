namespace FlowerShop.Application.Features.Users.Commands.UpdatePassword;

public record UpdatePasswordCommand
{
    public string UserId { get; set; } = null!;
    
    public string CurrentPassword { get; set; } = null!;
    
    public string NewPassword { get; set; } = null!;
    
    public string ConfirmPassword { get; set; } = null!;
};