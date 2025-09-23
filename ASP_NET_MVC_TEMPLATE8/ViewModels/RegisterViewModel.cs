using System.ComponentModel.DataAnnotations;

namespace ASP_NET_MVC_TEMPLATE8.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Unesite ime")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Ime mora biti izmedju 2 i 20 karaktera")]
    [Display(Name = "Ime")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Unesite prezime")]
    [StringLength(30, MinimumLength = 4, ErrorMessage = "Prezime mora biti izmedju 4 i 30 karaktera")]
    [Display(Name = "Prezime")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Unesite korisničko ime")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "Korisničko ime mora biti izmedju 4 i 10 karaktera")]
    [Display(Name = "Korisničko ime")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Unesite email adresu")]
    [EmailAddress(ErrorMessage = "Unesite validnu email adresu")]
    [Display(Name = "Email adresa")]
    public string Email { get; set; } = null!;
    

    [Required(ErrorMessage = "Unesite lozinku")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera")]
    [Display(Name = "Lozinka")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Potvrdite lozinku")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju")]
    [Display(Name = "Potvrda lozinke")]
    public string ConfirmPassword { get; set; } = null!;
}