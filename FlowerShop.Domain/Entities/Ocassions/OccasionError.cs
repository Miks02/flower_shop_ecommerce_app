using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Ocassions;

public static class OccasionError
{
    public static Error OccasionAlreadyExists(string name = "")
    {
        string message = string.IsNullOrWhiteSpace(name) 
            ? "Occasion with the same name already exists."
            : $"Occasion with the name '{name}' already exists.";
        
        return new Error("OccasionError_AlreadyExists", message);
    }
    
    public static Error OccasionNotFound(string identifier = "") 
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                ? "Occasion with the specified identifier does not exist."
                : $"Occasion with the identifier '{identifier}' has not been found.";
        
        return new Error("OccasionError_NotFound", message);
    }
    
    public static Error OccasionsNotFound(IReadOnlyList<int> occasionIds)
    {
        string message = $"Occasions with identifiers {string.Join(", ", occasionIds)} not found.";
        return new Error("OccasionError_NotFound", message);
    }
     
}