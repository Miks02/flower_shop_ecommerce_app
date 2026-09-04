
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Flowers;

public static class FlowerError
{
    public static Error FlowerAlreadyExists(string identifier = "")
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                    ? "Cvet sa istim nazivom već postoji."
                    : $"Cvet sa nazivom '{identifier}' već postoji.";
        
        return new Error("FlowerError_AlreadyExists", message);
    }
    
    public static Error FlowerNotFound(string identifier = "")   
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                        ? "Cvet sa navedenim identifikatorom ne postoji."
                        : $"Cvet sa identifikatorom '{identifier}' nije pronađen.";
        
        return new Error("FlowerError_NotFound", message);
    }
    
    public static Error InsufficientStock(string flowerName, int requestedQuantity, int availableStock)
    {
                string message = $"Nema dovoljno zaliha za cvet '{flowerName}'. Traženo: {requestedQuantity}, dostupno: {availableStock}.";
        return new Error("FlowerError_InsufficientStock", message);
    }

            public static Error InsufficientStock(IReadOnlyList<int> flowerIds)
            {
                string message = $"Nema dovoljno zaliha za cvetove sa identifikatorima {string.Join(", ", flowerIds)}.";
                return new Error("FlowerError_InsufficientStock", message);
            }

            public static Error FlowersNotFound(IReadOnlyList<int> flowerIds)
            {
                string message = $"Cvetovi sa identifikatorima {string.Join(", ", flowerIds)} nisu pronađeni.";
                return new Error("FlowerError_NotFound", message);
            }

            public static Error FlowerInUse(string identifier = "")
            {
                string message = string.IsNullOrWhiteSpace(identifier)
                    ? "Cvet se koristi u jednom ili više proizvoda i ne može se obrisati."
                    : $"Cvet '{identifier}' se koristi u jednom ili više proizvoda i ne može se obrisati.";

                return new Error("FlowerError_InUse", message);
            }
}