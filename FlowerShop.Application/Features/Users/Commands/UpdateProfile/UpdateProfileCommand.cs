using Microsoft.AspNetCore.Http;

namespace FlowerShop.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommand
{
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public IFormFile? ProfilePicture { get; set; }
    
}