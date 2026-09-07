using System.Security.Claims;
using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Cart.Queries.GetCart;
using FlowerShop.Application.Features.Loyalty.Queries.GetCurrentLoyaltyPoints;
using FlowerShop.Application.Features.Orders.Commands.CreateOrder;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

[Authorize(Roles = "User")]
public class CheckoutController(
    IUserProvider userProvider,
    GetCartHandler getCartHandler,
    CreateOrderHandler createOrderHandler,
    GetCurrentLoyaltyPointsHandler getCurrentLoyaltyPointsHandler,
    ILogger<CheckoutController> logger) : BaseController(logger)
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();
        var cart = await getCartHandler.Handle(new GetCartQuery { UserId = userId }, ct);

        if (cart.ItemCount == 0)
        {
            SetErrorMessage("Vaša korpa je prazna. Dodajte artikle u korpu pre odlaska na plaćanje.");
            return RedirectToAction("Index", "Catalogue");
        }

        var userDetails = await userProvider.GetCurrentUserDetails(userId, ct);

        var viewModel = new CheckoutViewModel
        {
            BuyerEmail = userDetails.Email,
            BuyerPhone = userDetails.PhoneNumber,
            RecipientFullName = $"{userDetails.FirstName} {userDetails.LastName}".Trim(),
            RecipientPhoneNumber = userDetails.PhoneNumber,
            OrderAddress = userDetails.Address,
            DeliveryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            DeliveryTime = new TimeOnly(12, 0),
            LoyaltyPoints = await getCurrentLoyaltyPointsHandler.Handle(new GetCurrentLoyaltyPointsQuery(userId), ct),
            Cart = cart
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model, CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();
        var cart = await getCartHandler.Handle(new GetCartQuery { UserId = userId }, ct);

        if (cart.ItemCount == 0)
        {
            SetErrorMessage("Vaša korpa je prazna.");
            return RedirectToAction("Index", "Catalogue");
        }

        if (!ModelState.IsValid)
        {
            var invalidVm = model with { Cart = cart };
            return View(invalidVm);
        }

        var deliveryDate = model.DeliveryDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var deliveryTime = model.DeliveryTime ?? new TimeOnly(12, 0);
        var orderDate = deliveryDate.ToDateTime(deliveryTime);

        var command = new CreateOrderCommand
        {
            BuyerId = userId,
            RecipientFullName = model.RecipientFullName,
            RecipientPhoneNumber = model.RecipientPhoneNumber,
            OrderAddress = model.OrderAddress,
            City = model.City,
            ZipCode = model.ZipCode,
            Note = model.Note,
            OrderDate = orderDate,
            UseLoyaltyPoints = model.UseLoyaltyPoints,
            OrderItems = cart.Items.Select(i => new CreateOrderCommand.OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImagePath = i.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.Price
            }).ToList()
        };

        var result = await createOrderHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            SetErrorMessage(result.Errors[0].Description);

            var invalidVm = model with { Cart = cart };
            return View(invalidVm);
        }

        return RedirectToAction("Index", "Orders", new { area = "User", orderId = result.Payload!.OrderId });
    }
}