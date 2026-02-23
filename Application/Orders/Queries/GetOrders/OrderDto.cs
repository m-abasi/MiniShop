using Domain.Entities;

namespace Application.Orders.Queries.GetOrders;

public class OrderDto
{
    public int Id { get; set; }
    public string UserEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public string ShippingAddress { get; set; } = null!;
    public string ShippingCity { get; set; } = null!;
    public string ShippingZip { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}
