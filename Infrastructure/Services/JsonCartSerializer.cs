using System.Text.Json;
using Application.Cart;

namespace Infrastructure.Services;

/// <summary>
/// SRP: فقط مسئول سریالایز/دسریالایز با JSON است.
/// OCP: اگر فردا بخواهیم MessagePack استفاده کنیم، فقط یک کلاس جدید می‌سازیم.
/// </summary>
public class JsonCartSerializer : ICartSerializer
{
    public string Serialize(List<CartItem> cart)
        => JsonSerializer.Serialize(cart);

    public List<CartItem> Deserialize(string? json)
        => string.IsNullOrEmpty(json)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
}
