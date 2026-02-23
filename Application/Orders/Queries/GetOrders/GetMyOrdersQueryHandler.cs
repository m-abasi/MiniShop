using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniShop.Application.Common.Interfaces;

namespace Application.Orders.Queries.GetOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, List<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMyOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .Where(o => o.UserId == request.UserId)
            .OrderByDescending(o => o.CreatedAt)
            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
