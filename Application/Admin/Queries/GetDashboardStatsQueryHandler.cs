using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MiniShop.Application.Common.Interfaces;

namespace Application.Admin.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        return new DashboardStatsDto
        {
            TotalProducts = await _context.Products.CountAsync(cancellationToken),
            TotalOrders = await _context.Orders.CountAsync(cancellationToken),
            PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken),
            TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice, cancellationToken),
            TotalUsers = _userManager.Users.Count(),
            LowStockProducts = await _context.Products.CountAsync(p => p.Stock <= 5, cancellationToken)
        };
    }
}
