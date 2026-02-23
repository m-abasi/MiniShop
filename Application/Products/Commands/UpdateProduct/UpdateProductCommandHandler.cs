using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniShop.Application.Common.Interfaces;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IRedisService _redis;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IApplicationDbContext context, IRedisService redis, ILogger<UpdateProductCommandHandler> logger)
    {
        _context = context;
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("UpdateProduct: Product {Id} not found.", request.Id);
            return false;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        // Cache Invalidation
        await _redis.RemoveAsync("products:all");

        _logger.LogInformation("Product {Id} updated.", request.Id);
        return true;
    }
}
