using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Carts;

public static class CartError
{
    public static Error CartNotFound()
        => new("CartError_NotFound", "Korpa nije pronađena.");

    public static Error CartItemNotFound()
        => new("CartError_ItemNotFound", "Stavka korpe nije pronađena.");

    public static Error CartLimitReached()
        => new("CartError_LimitReached", $"Korpa može imati najviše {Cart.MaxDistinctItems} različitih proizvoda.");

    public static Error InsufficientStock(string productName, int requested, int available)
        => new("CartError_InsufficientStock",
            $"Nema dovoljno zaliha za '{productName}'. Traženo: {requested}, dostupno: {available}.");

    public static Error InvalidQuantity()
        => new("CartError_InvalidQuantity", "Količina mora biti najmanje 1.");
}
