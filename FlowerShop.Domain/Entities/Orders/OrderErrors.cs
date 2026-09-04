using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Orders;

public static class OrderError
{
    public static Error OrderNotFound(string? identifier = null)
    {
        string message = string.IsNullOrWhiteSpace(identifier)
            ? "Porudžbina sa navedenim identifikatorom ne postoji."
            : $"Porudžbina sa identifikatorom '{identifier}' nije pronađena.";

        return new Error("OrderError_NotFound", message);
    }

    public static Error OrderItemNotFound(string? identifier = null)
    {
        string message = string.IsNullOrWhiteSpace(identifier)
            ? "Stavka porudžbine nije pronađena."
            : $"Stavka porudžbine sa identifikatorom '{identifier}' nije pronađena.";

        return new Error("OrderError_ItemNotFound", message);
    }

    public static Error EmptyOrder()
        => new("OrderError_Empty", "Porudžbina mora sadržati najmanje jednu stavku.");

    public static Error InvalidQuantity()
        => new("OrderError_InvalidQuantity", "Količina mora biti najmanje 1.");

    public static Error InvalidStatusTransition(OrderStatus currentStatus, OrderStatus targetStatus)
        => new("OrderError_InvalidStatusTransition",
            $"Nije moguće promeniti status porudžbine iz '{currentStatus}' u '{targetStatus}'.");

    public static Error InvalidDeliveryStatusTransition(DeliveryStatus currentStatus, DeliveryStatus targetStatus)
        => new("OrderError_InvalidDeliveryStatusTransition",
            $"Nije moguće promeniti status dostave iz '{currentStatus}' u '{targetStatus}'.");

    public static Error CannotCancel(OrderStatus status)
        => new("OrderError_CannotCancel",
            $"Porudžbina sa statusom '{status}' ne može biti otkazana.");

    public static Error CannotModify(OrderStatus status)
        => new("OrderError_CannotModify",
            $"Porudžbina sa statusom '{status}' ne može biti izmenjena.");

    public static Error AlreadyCancelled(string? identifier = null)
    {
        string message = string.IsNullOrWhiteSpace(identifier)
            ? "Porudžbina je već otkazana."
            : $"Porudžbina '{identifier}' je već otkazana.";

        return new Error("OrderError_AlreadyCancelled", message);
    }

    public static Error AlreadyCompleted(string? identifier = null)
    {
        string message = string.IsNullOrWhiteSpace(identifier)
            ? "Porudžbina je već završena."
            : $"Porudžbina '{identifier}' je već završena.";

        return new Error("OrderError_AlreadyCompleted", message);
    }
    
}
