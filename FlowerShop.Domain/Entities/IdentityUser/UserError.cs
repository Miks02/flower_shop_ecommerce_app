using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.IdentityUser;

public class UserError
{
    public static Error EmailAlreadyExists(string email = "")
    {
        string message = string.IsNullOrWhiteSpace(email)
            ? "Email is taken"
            : $"Email '{email}' is taken";
            
        return new Error("User.EmailAlreadyExists", message);
    }

    public static Error UsernameAlreadyExists(string username = "")
    {
        string message = string.IsNullOrWhiteSpace(username)
            ? "Username is taken"
            : $"Email is {username}";

        return new Error("User.UsernameAlreadyExists", message);
    }

    public static Error NotFound(string identifier = "")
    {
        string message = string.IsNullOrWhiteSpace(identifier)
            ? "User not found"
            : $"User with identifier '{identifier}' is not found";

        return new Error("User.NotFound", message);
    }
}