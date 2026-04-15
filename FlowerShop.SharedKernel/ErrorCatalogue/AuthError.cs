using FlowerShop.SharedKernel.Results;

namespace FlowerShop.SharedKernel.ErrorCatalogue;

public class AuthError
{
    public static Error RegistrationFailed(string message = "Unexpected error happened during registration")
        => new("Auth.RegistrationFailed", message);
        
    public static Error LoginFailed(string message = "Unexpected error happened during login")
        => new("Auth.LoginFailed", message);
        
    public static Error PasswordError(string message = "Error occurred while trying to assign password to the user")
        => new("Auth.InvalidCredentials", message);

    public static Error InvalidCurrentPassword(
        string message = "Entered password does not match the current password")
        => new("Auth.InvalidCurrentPassword", message);

    public static Error PasswordTooShort(string message = "Password is too short")
        => new("Auth.PasswordTooShort", message);

    public static Error PasswordRequiresDigit(string message = "Password must contain at least one digit ('0'-'9')")
        => new("Auth.PasswordRequiresDigit", message);

    public static Error PasswordRequiresUpper(string message = "Password must contain at least one uppercase letter ('A'-'Z')")
        => new("Auth.PasswordRequiresUpper", message);

    public static Error PasswordRequiresNonAlphanumeric(string message = "Password must contain at least one special character")
        => new("Auth.PasswordRequiresNonAlphanumeric", message);

    public static Error AccountLocked(string message = "Account is locked")
        => new("Auth.AccountLocked", message);
    
    public static Error InvalidRole(string message = "Invalid role has been entered")
        => new("Auth.InvalidRole", message);
}