using FlowerShop.Models;

namespace FlowerShop.Services.Results;

public class ProfileUpdateResult : OperationResult<ApplicationUser>
{
    public bool ProfileUpdated { get; set; }
    
    public bool PasswordChanged { get; set; }
}