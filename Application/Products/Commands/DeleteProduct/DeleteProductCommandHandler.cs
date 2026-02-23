using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniShop.Application.Common.Interfaces;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IRedisService _redis;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IApplicationDbContext context, IRedisService redis, ILogger<DeleteProductCommandHandler> logger)
    {
        _context = context;
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("DeleteProduct: Product with Id {Id} not found.", request.Id);
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Cache Invalidation
        await _redis.RemoveAsync("products:all");

        _logger.LogInformation("Product {Id} deleted.", request.Id);
        return true;
    }
}
