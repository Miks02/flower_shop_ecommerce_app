using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Ocassions;

public static class OccasionError
{
    public static Error OccasionAlreadyExists(string name = "")
    {
        string message = string.IsNullOrWhiteSpace(name) 
                    ? "Prilika sa istim nazivom već postoji."
                    : $"Prilika sa nazivom '{name}' već postoji.";
        
        return new Error("OccasionError_AlreadyExists", message);
    }
    
    public static Error OccasionNotFound(string identifier = "") 
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                        ? "Prilika sa navedenim identifikatorom ne postoji."
                        : $"Prilika sa identifikatorom '{identifier}' nije pronađena.";
        
        return new Error("OccasionError_NotFound", message);
    }
    
    public static Error OccasionsNotFound(IReadOnlyList<int> occasionIds)
    {
                string message = $"Prilike sa identifikatorima {string.Join(", ", occasionIds)} nisu pronađene.";
        return new Error("OccasionError_NotFound", message);
    }
     
}