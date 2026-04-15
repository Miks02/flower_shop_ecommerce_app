using FlowerShop.SharedKernel.Results;

namespace FlowerShop.SharedKernel.ErrorCatalogue;

public class GeneralError
{
    public static Error NotFound(string message = "The requested resource was not found")
        => new("General.NotFound", message);

    public static Error InternalServerError(string message = "Internal server error")
        => new("General.InternalServerError", message);

    public static Error UnknownError(string message = "An unknown error occurred")
        => new("General.UnknownError", message);

    public static Error LimitReached(string message = "Limit for this request has been reached")
        => new("General.LimitReached", message);
    
    public static Error Conflict(string message = "Conflict occurred")
        => new("General.Conflict", message);
}