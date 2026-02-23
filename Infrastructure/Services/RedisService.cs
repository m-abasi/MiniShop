using Application.Common.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(string key, string value, TimeSpan ttl)
        => await _db.StringSetAsync(key, value, ttl);

    public async Task RemoveAsync(string key)
        => await _db.KeyDeleteAsync(key);

    public async Task<long> IncrementAsync(string key, TimeSpan ttl)
    {
        var count = await _db.StringIncrementAsync(key);
        // TTL فقط در اولین increment اعمال می‌شود
        if (count == 1)
            await _db.KeyExpireAsync(key, ttl);
        return count;
    }
}
