using FlowerShop.Models;

namespace FlowerShop.Dto.User;

public class ProfileUpdateDto
{
    public bool ProfileUpdated { get; set; }
    
    public bool PasswordChanged { get; set; }

    public ApplicationUser User { get; set; } = null!;

}