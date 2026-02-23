using MediatR;
using Microsoft.Extensions.Logging;
using MiniShop.Application.Common.Interfaces;

namespace Application.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(IApplicationDbContext context, ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order is null) return false;

        order.Status = request.NewStatus;
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Order {Id} status changed to {Status}.", request.OrderId, request.NewStatus);
        return true;
    }
}
