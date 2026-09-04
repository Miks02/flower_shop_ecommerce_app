using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Cart.Commands.AddToCart;
using FlowerShop.Application.Features.Cart.Commands.RemoveCartItem;
using FlowerShop.Application.Features.Cart.Queries.GetCart;
using FlowerShop.Infrastructure.Htmx;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

[Authorize(Roles = "User")]
public class CartController(
    ILogger<CartController> logger,
    IUserProvider userProvider,
    GetCartHandler getCartHandler,
    AddToCartHandler addToCartHandler,
    RemoveCartItemHandler removeCartItemHandler) : BaseController(logger)
{
    [HttpGet]
    public async Task<IActionResult> Menu(CancellationToken ct = default)
    {
        var cart = await getCartHandler.Handle(new GetCartQuery { UserId = userProvider.GetCurrentUserId() }, ct);
        return PartialView("_CartMenu", cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1, CancellationToken ct = default)
    {
        var result = await addToCartHandler.Handle(new AddToCartCommand
        {
            UserId = userProvider.GetCurrentUserId(),
            ProductId = productId,
            Quantity = quantity
        }, ct);

        if (!result.IsSuccess)
        {
            Response.ShowError(result.Errors[0].Description);
            return Request.IsHtmx() ? NoContent() : RedirectToAction("Index", "Catalogue");
        }

        Response.ShowSuccess("Proizvod je dodat u korpu.");
        return Request.IsHtmx() ? NoContent() : RedirectToAction(nameof(Menu));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id, CancellationToken ct = default)
    {
        var result = await removeCartItemHandler.Handle(new RemoveCartItemCommand
        {
            UserId = userProvider.GetCurrentUserId(),
            CartItemId = id
        }, ct);

        if (!result.IsSuccess)
            Response.ShowError(result.Errors[0].Description);

        var cart = await getCartHandler.Handle(new GetCartQuery { UserId = userProvider.GetCurrentUserId() }, ct);
        return PartialView("_CartMenu", cart);
    }
}
