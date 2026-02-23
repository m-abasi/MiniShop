using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        [MaxLength(500)]
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    }
}
