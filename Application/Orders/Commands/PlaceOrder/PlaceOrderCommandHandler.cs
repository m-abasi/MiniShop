using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniShop.Application.Common.Interfaces;

namespace Application.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IRedisService _redis;
    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(IApplicationDbContext context, IRedisService redis, ILogger<PlaceOrderCommandHandler> logger)
    {
        _context = context;
        _redis = redis;
        _logger = logger;
    }

    public async Task<int> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
            throw new InvalidOperationException("Cannot place an empty order.");

        // Validate stock for all items
        foreach (var item in request.Items)
        {
            var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken)
                          ?? throw new Exception($"Product {item.ProductId} not found.");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for '{product.Name}'. Available: {product.Stock}");
        }

        // Deduct stock
        foreach (var item in request.Items)
        {
            var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
            product!.Stock -= item.Quantity;
        }

        var order = new Order
        {
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = request.ShippingAddress,
            ShippingCity = request.ShippingCity,
            ShippingZip = request.ShippingZip,
            TotalPrice = request.Items.Sum(i => i.UnitPrice * i.Quantity),
            OrderItems = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Cache Invalidation — موجودی محصولات تغییر کرد
        await _redis.RemoveAsync("products:all");

        _logger.LogInformation("Order {OrderId} placed for user {UserId}. Total: {Total}", order.Id, request.UserId, order.TotalPrice);
        return order.Id;
    }
}
