using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Categories;

public static class CategoryError
{
    public static Error CategoryAlreadyExists(string name = "")
    {
        string message = string.IsNullOrWhiteSpace(name)
                    ? "Kategorija sa istim nazivom već postoji."
                    : $"Kategorija sa nazivom '{name}' već postoji.";
        
        return new Error("CategoryError_AlreadyExists", message);
    }
    
    
    public static Error CategoryNotFound(string identifier = "")    
    {
        string message = string.IsNullOrWhiteSpace(identifier)
                        ? "Kategorija sa navedenim identifikatorom ne postoji."
                        : $"Kategorija sa identifikatorom '{identifier}' nije pronađena.";
        
        return new Error("CategoryNotFound", message);
    }
}