using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Deliverers;

public static class DelivererError
{
    public static Error NotFound(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("DelivererError_NotFound", "Deliverer not found.") 
            : new Error("DelivererError_NotFound", $"Deliverer with id '{id}' was not found.");
    
    public static Error DelivererUnavailable(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("DelivererError_Unavailable", "Deliverer is unavailable at the moment.") 
            : new Error("DelivererError_Unavailable", $"Deliverer with id '{id}' is unavailable at the moment.");
    
    public static Error MinAmountOfProductsNotReached(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("DelivererError_MinProductsNotReached", "Deliverer has not reached the minimum amount of products.") 
            : new Error("DelivererError_MinProductsNotReached", $"Deliverer with id '{id}' has not reached the minimum amount of products.");

    public static Error CannotDeleteWhileOnDuty(string? id = null)
        => string.IsNullOrWhiteSpace(id)
            ? new Error("DelivererError_CannotDeleteWhileOnDuty", "Dostavljač ne može biti obrisan dok je na dostavi.")
            : new Error("DelivererError_CannotDeleteWhileOnDuty", $"Dostavljač '{id}' ne može biti obrisan dok je na dostavi.");

    public static Error EmailAlreadyExists(string email)
        => new Error("DelivererError_EmailAlreadyExists", $"Korisnik sa email adresom '{email}' već postoji.");
}