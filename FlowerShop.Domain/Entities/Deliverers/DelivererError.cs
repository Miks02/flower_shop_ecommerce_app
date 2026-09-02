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
    
    public static Error MaxAmountOfOrdersReached(string? id = null) 
        => string.IsNullOrWhiteSpace(id) 
            ? new Error("Deliverer.MaxOrdersReached", "Deliverer has reached the maximum amount of orders.") 
            : new Error("Deliverer.MaxOrdersReached", $"Deliverer with id '{id}' has reached the maximum amount of orders.");
}