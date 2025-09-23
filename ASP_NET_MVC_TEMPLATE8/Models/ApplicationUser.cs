using Microsoft.AspNetCore.Identity;

namespace ASP_NET_MVC_TEMPLATE8.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    
}