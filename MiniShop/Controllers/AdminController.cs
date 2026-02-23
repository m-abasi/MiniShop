using Application.Admin.Queries;
using Application.Orders.Commands.UpdateOrderStatus;
using Application.Orders.Queries.GetOrders;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    // GET: /Admin
    public async Task<IActionResult> Index()
    {
        var stats = await _mediator.Send(new GetDashboardStatsQuery());
        return View(stats);
    }

    // GET: /Admin/Orders
    public async Task<IActionResult> Orders()
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery());
        return View(orders);
    }

    // GET: /Admin/OrderDetails/5
    public async Task<IActionResult> OrderDetails(int id)
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery());
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return NotFound();
        return View(order);
    }

    // POST: /Admin/UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        await _mediator.Send(new UpdateOrderStatusCommand(orderId, newStatus));
        return RedirectToAction(nameof(Orders));
    }
}
