using Domain.Entities;
using MediatR;

namespace Application.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(int OrderId, OrderStatus NewStatus) : IRequest<bool>;
