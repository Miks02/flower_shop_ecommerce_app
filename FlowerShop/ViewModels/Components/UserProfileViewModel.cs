using FlowerShop.Enums;

namespace FlowerShop.ViewModels.Components;

public class UserProfileViewModel
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    public string? ProfilePicture { get; set; }
    
    public string Address { get; set; } = null!;
    
    public AccountStatus Status { get; set; }
    
    public string RegistrationDate { get; set; } = null!;
    
    public int TotalOrders { get; set; } = 0;
    
    public int LoyaltyPoints { get; set; } = 0;
    
    public int UpcomingOrders { get; set; } = 0;
}