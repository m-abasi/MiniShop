using Application.Cart;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
/// SRP: فقط مسئول خواندن/نوشتن در Session است.
/// LSP: اگر فردا RedisCartStorage جایگزین شود، CartService بدون تغییر کار می‌کند.
/// DIP: CartService به این کلاس وابسته نیست، به ICartStorage وابسته است.
/// </summary>
public class SessionCartStorage : ICartStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionCartStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public string? Read(string key) => Session.GetString(key);

    public void Write(string key, string value) => Session.SetString(key, value);

    public void Remove(string key) => Session.Remove(key);
}
