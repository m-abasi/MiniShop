using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
