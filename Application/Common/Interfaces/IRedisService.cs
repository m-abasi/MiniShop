namespace Application.Common.Interfaces;

/// <summary>
/// Abstraction برای Redis — Application layer به این وابسته است، نه به StackExchange.Redis مستقیم
/// </summary>
public interface IRedisService
{
    // Cache
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value, TimeSpan ttl);
    Task RemoveAsync(string key);

    // Rate Limiting (Counter)
    /// <summary>مقدار counter را یک واحد افزایش می‌دهد. TTL فقط در اولین ست شدن اعمال می‌شود.</summary>
    Task<long> IncrementAsync(string key, TimeSpan ttl);
}
