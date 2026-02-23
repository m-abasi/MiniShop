using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Shipping info
        public string ShippingAddress { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingZip { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
