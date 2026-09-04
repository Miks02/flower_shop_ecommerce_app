using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.IdentityUser;

public class UserError
{
    public static Error EmailAlreadyExists(string email = "")
    {
        string message = string.IsNullOrWhiteSpace(email)
                    ? "Email adresa je zauzeta."
                    : $"Email adresa '{email}' je zauzeta.";
            
        return new Error("User.EmailAlreadyExists", message);
    }

    public static Error UsernameAlreadyExists(string username = "")
    {
        string message = string.IsNullOrWhiteSpace(username)
                    ? "Korisničko ime je zauzeto."
                    : $"Korisničko ime '{username}' je zauzeto.";

        return new Error("User.UsernameAlreadyExists", message);
    }

    public static Error NotFound(string identifier = "")
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                    ? "Korisnik nije pronađen."
                    : $"Korisnik sa identifikatorom '{identifier}' nije pronađen.";

        return new Error("User.NotFound", message);
    }
}