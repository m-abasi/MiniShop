using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
