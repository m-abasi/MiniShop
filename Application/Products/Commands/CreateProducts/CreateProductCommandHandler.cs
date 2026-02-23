using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniShop.Application.Common.Interfaces;

namespace Application.Products.Commands.CreateProducts;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IRedisService _redis;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IApplicationDbContext context, IRedisService redis, ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _redis = redis;
        _logger = logger;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            _logger.LogWarning("CreateProduct: Category {CategoryId} not found.", request.CategoryId);
            throw new Exception("Category not found");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Cache Invalidation
        await _redis.RemoveAsync("products:all");

        _logger.LogInformation("Product created with Id {Id}.", product.Id);
        return product.Id;
    }
}
