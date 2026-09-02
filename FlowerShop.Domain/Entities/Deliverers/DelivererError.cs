using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Deliverers;

public static class DelivererError
{
    public static Error NotFound(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("Deliverer.NotFound", "Deliverer not found.") 
            : new Error("Deliverer.NotFound", $"Deliverer with id '{id}' was not found.");
    
    public static Error DelivererUnavailable(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("Deliverer.Unavailable", "Deliverer is unavailable at the moment.") 
            : new Error("Deliverer.Unavailable", $"Deliverer with id '{id}' is unavailable at the moment.");
    
    public static Error MinAmountOfProductsNotReached(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("Deliverer.MinProductsNotReached", "Deliverer has not reached the minimum amount of products.") 
            : new Error("Deliverer.MinProductsNotReached", $"Deliverer with id '{id}' has not reached the minimum amount of products.");
}