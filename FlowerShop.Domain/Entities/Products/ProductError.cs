using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Products;

public static class ProductError
{
    public static Error ProductAlreadyExists(string name = "")
    {
        string message = string.IsNullOrWhiteSpace(name) 
                    ? "Proizvod sa istim nazivom već postoji."
                    : $"Proizvod sa nazivom '{name}' već postoji.";
        
        return new Error("ProductError_AlreadyExists", message);
    }
    
    public static Error ProductNotFound(int? identifier) 
    {
        string message = identifier is null
                    ? "Proizvod sa navedenim identifikatorom ne postoji."
                    : $"Proizvod sa identifikatorom '{identifier}' nije pronađen.";
        
        return new Error("ProductError_NotFound", message);
    }
    
     
}