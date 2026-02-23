using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniShop.Application.Common.Interfaces;
using System.Text.Json;

namespace Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRedisService _redis;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    private const string CacheKey = "products:all";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public GetProductsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IRedisService redis,
        ILogger<GetProductsQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _redis = redis;
        _logger = logger;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Cache-Aside: ابتدا Redis را چک کن
        var cached = await _redis.GetAsync(CacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Products served from Redis cache.");
            return JsonSerializer.Deserialize<List<ProductDto>>(cached) ?? new();
        }

        // Cache Miss: از DB بخوان
        _logger.LogInformation("Cache miss — fetching products from database.");
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        // در Cache ذخیره کن
        await _redis.SetAsync(CacheKey, JsonSerializer.Serialize(products), CacheTtl);

        return products;
    }
}
