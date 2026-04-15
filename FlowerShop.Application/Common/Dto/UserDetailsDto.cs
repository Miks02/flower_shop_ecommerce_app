using FlowerShop.Domain.Enums;

namespace FlowerShop.Application.Common.Abstractions.Dto;

public record UserDetailsDto
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    public string? ProfilePicture { get; set; }
    
    public string Address { get; set; } = null!;
    
    public AccountStatus Status { get; set; }
    
    public DateTime RegistrationDate { get; set; }
    
    public int TotalOrders { get; set; } = 0;
    
    public int LoyaltyPoints { get; set; } = 0;
    
    public int UpcomingOrders { get; set; } = 0;
};