using FlowerShop.Application.Features.Cart.Queries.GetCart;

namespace FlowerShop.Web.ViewModels;

public record CheckoutViewModel
{
    public string RecipientFullName { get; init; } = string.Empty;
    public string RecipientPhoneNumber { get; init; } = string.Empty;
    public string OrderAddress { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public DateOnly? DeliveryDate { get; init; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
    public TimeOnly? DeliveryTime { get; init; } = new TimeOnly(12, 0);
    public string? Note { get; init; }

    public string BuyerEmail { get; init; } = string.Empty;
    public string BuyerPhone { get; init; } = string.Empty;

    public string CardNumber { get; init; } = string.Empty;
    public string CardExpiry { get; init; } = string.Empty;
    public string CardCvc { get; init; } = string.Empty;
    public string CardHolder { get; init; } = string.Empty;
    public bool AcceptTerms { get; init; }

    public GetCartResponse Cart { get; init; } = new();
    public decimal DeliveryFee { get; init; } = 300m;
    public decimal Subtotal => Cart.Total;
    public decimal Total => Subtotal + (Cart.ItemCount > 0 ? DeliveryFee : 0m);
}