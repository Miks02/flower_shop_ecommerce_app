using System.ComponentModel.DataAnnotations;

namespace FlowerShop.ViewModels;

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