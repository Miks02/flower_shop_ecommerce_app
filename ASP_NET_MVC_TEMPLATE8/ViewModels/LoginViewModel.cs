using System.ComponentModel.DataAnnotations;

namespace ASP_NET_MVC_TEMPLATE8.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Unesite korisničko ime")]
    [Display(Name = "Korisničko ime")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Unesite lozinku")]
    [Display(Name = "Lozinka")]
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "Zapamti me")]
    public bool RememberMe { get; set; }
}