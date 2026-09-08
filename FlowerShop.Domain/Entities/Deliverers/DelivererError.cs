using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Deliverers;

public static class DelivererError
{
    public static Error NotFound(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
                ? new Error("DelivererError_NotFound", "Dostavljač nije pronađen.") 
                : new Error("DelivererError_NotFound", $"Dostavljač sa identifikatorom '{id}' nije pronađen.");
    
    public static Error DelivererUnavailable(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
                ? new Error("DelivererError_Unavailable", "Dostavljač trenutno nije dostupan.") 
                : new Error("DelivererError_Unavailable", $"Dostavljač sa identifikatorom '{id}' trenutno nije dostupan.");

    public static Error MinAmountOfProductsNotReached()
        => new Error("DelivererError_MinProductsNotReached",
            "Broj poručenih artikala je previše mali za dostavljača sa ovim tipom vozila."); 

    public static Error CannotDeleteWhileOnDuty(string? id = null)
        => string.IsNullOrWhiteSpace(id)
            ? new Error("DelivererError_CannotDeleteWhileOnDuty", "Dostavljač ne može biti obrisan dok je na dostavi.")
            : new Error("DelivererError_CannotDeleteWhileOnDuty", $"Dostavljač '{id}' ne može biti obrisan dok je na dostavi.");

    public static Error EmailAlreadyExists(string email)
        => new Error("DelivererError_EmailAlreadyExists", $"Korisnik sa email adresom '{email}' već postoji.");
}