using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiniShop.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
