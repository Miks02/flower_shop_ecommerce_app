using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Products;

public static class ProductError
{
    public static Error ProductAlreadyExists(string name = "")
    {
        string message = string.IsNullOrWhiteSpace(name) 
            ? "Product with the same name already exists."
            : $"Product with the name '{name}' already exists.";
        
        return new Error("ProductError_AlreadyExists", message);
    }
    
    public static Error ProductNotFound(int? identifier) 
    {
        string message = identifier is null
            ? "Product with the specified identifier does not exist."
            : $"Product with the identifier '{identifier}' has not been found.";
        
        return new Error("ProductError_NotFound", message);
    }
    
     
}