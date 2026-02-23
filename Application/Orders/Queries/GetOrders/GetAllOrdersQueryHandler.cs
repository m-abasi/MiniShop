using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniShop.Application.Common.Interfaces;

namespace Application.Orders.Queries.GetOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
