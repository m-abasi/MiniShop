using MediatR;

namespace Application.Orders.Queries.GetOrders;

public record GetAllOrdersQuery() : IRequest<List<OrderDto>>;
