namespace FlowerShop.ViewModels.Components;

public class ProfileSettingsViewModel
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;

    public string FullNameInitials { get; set; } = null!;
    
    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;
    
    public IFormFile? ProfilePicture { get; set; }
    
    public string? ImagePath { get; set; }
}