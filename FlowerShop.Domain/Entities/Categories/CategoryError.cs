using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Categories;

public static class CategoryError
{
    public static Error CategoryAlreadyExists(string name = "")
    {
        string message = string.IsNullOrWhiteSpace(name)
            ? "A category with the same name already exists."
            : $"A category with the name '{name}' already exists.";
        
        return new Error("CategoryError_AlreadyExists", message);
    }
    
    
    public static Error CategoryNotFound(string identifier = "")    
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                ? "A category with the specified identifier does not exist."
                : $"A category with the identifier '{identifier}' has not been found.";
        
        return new Error("CategoryNotFound", message);
    }
}