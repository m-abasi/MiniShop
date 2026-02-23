using MediatR;

namespace Application.Orders.Queries.GetOrders;

public record GetMyOrdersQuery(string UserId) : IRequest<List<OrderDto>>;
