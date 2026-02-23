using Application.Cart;
using Application.Orders.Commands.PlaceOrder;
using Application.Orders.Queries.GetOrders;
using Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniShop.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICartService _cart;

    public OrderController(IMediator mediator, ICartService cart)
    {
        _mediator = mediator;
        _cart = cart;
    }

    // GET: /Order/Checkout
    public IActionResult Checkout()
    {
        var items = _cart.GetCart();
        if (!items.Any())
            return RedirectToAction("Index", "Cart");

        ViewBag.CartItems = items;
        ViewBag.Total = _cart.GetTotal();
        return View(new CheckoutViewModel());
    }

    // POST: /Order/Checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var items = _cart.GetCart();
        if (!items.Any())
            return RedirectToAction("Index", "Cart");

        if (!ModelState.IsValid)
        {
            ViewBag.CartItems = items;
            ViewBag.Total = _cart.GetTotal();
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var orderId = await _mediator.Send(new PlaceOrderCommand(
                userId,
                model.ShippingAddress,
                model.ShippingCity,
                model.ShippingZip,
                items
            ));

            _cart.Clear();
            return RedirectToAction(nameof(Confirmation), new { id = orderId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.CartItems = items;
            ViewBag.Total = _cart.GetTotal();
            return View(model);
        }
    }

    // GET: /Order/Confirmation/5
    public async Task<IActionResult> Confirmation(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _mediator.Send(new GetMyOrdersQuery(userId));
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return NotFound();
        return View(order);
    }

    // GET: /Order/MyOrders
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _mediator.Send(new GetMyOrdersQuery(userId));
        return View(orders);
    }
}
