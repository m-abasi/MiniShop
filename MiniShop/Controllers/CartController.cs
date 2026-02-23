using Application.Cart;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Products.Queries.GetProductById;

namespace MiniShop.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cart;
    private readonly IMediator _mediator;

    public CartController(ICartService cart, IMediator mediator)
    {
        _cart = cart;
        _mediator = mediator;
    }

    public IActionResult Index()
    {
        var items = _cart.GetCart();
        ViewBag.Total = _cart.GetTotal();
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(productId));
        if (product is null)
            return NotFound();

        if (product.Stock < quantity)
        {
            TempData["Error"] = $"Only {product.Stock} units available.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        _cart.AddItem(productId, product.Name, product.Price, quantity);
        TempData["Success"] = $"'{product.Name}' added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(int productId)
    {
        _cart.RemoveItem(productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        _cart.Clear();
        return RedirectToAction(nameof(Index));
    }
}
