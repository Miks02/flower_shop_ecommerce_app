
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Flowers;

public static class FlowerError
{
    public static Error FlowerAlreadyExists(string identifier = "")
    {
        string message = string.IsNullOrWhiteSpace(identifier)
            ? "A flower with the same name already exists."
            : $"A flower with the name '{identifier}' already exists.";
        
        return new Error("FlowerError_AlreadyExists", message);
    }
    
    public static Error FlowerNotFound(string identifier = "")   
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                ? "A flower with the specified identifier does not exist."
                : $"A flower with the identifier '{identifier}' has not been found.";
        
        return new Error("FlowerError_NotFound", message);
    }
    
    public static Error InsufficientStock(string flowerName, int requestedQuantity, int availableStock)
    {
        string message = $"Insufficient stock for flower '{flowerName}'. Requested: {requestedQuantity}, Available: {availableStock}.";
        return new Error("FlowerError_InsufficientStock", message);
    }

    public static Error FlowersNotFound(IReadOnlyList<int> flowerIds)
    {
        string message = $"Flowers with identifiers {string.Join(", ", flowerIds)} not found.";
        return new Error("FlowerError_NotFound", message);
    }
}