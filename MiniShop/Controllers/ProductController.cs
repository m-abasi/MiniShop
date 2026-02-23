using Application.Categories.Queries.GetCategories;
using Application.Products.Commands.CreateProducts;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiniShop.Controllers;

public class ProductController : Controller
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: /Product
    public async Task<IActionResult> Index()
    {
        var products = await _mediator.Send(new GetProductsQuery());
        return View(products);
    }

    // GET: /Product/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View();
    }

    // POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync();
            return View(command);
        }

        await _mediator.Send(command);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Product/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null) return NotFound();

        await PopulateCategoriesAsync();
        var command = new UpdateProductCommand(product.Id, product.Name, product.Description, product.Price, product.Stock, 0);
        return View(command);
    }

    // POST: /Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(UpdateProductCommand command)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync();
            return View(command);
        }

        var success = await _mediator.Send(command);
        if (!success) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null) return NotFound();
        return View(product);
    }

    // POST: /Product/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesAsync()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
    }
}
