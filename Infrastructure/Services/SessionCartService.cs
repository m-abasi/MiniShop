using Application.Cart;

namespace Infrastructure.Services;

/// <summary>
/// بازطراحی شده بر اساس اصول SOLID:
///
/// S — SRP: این کلاس فقط منطق سبد خرید را مدیریت می‌کند.
///          نه سریالایز (ICartSerializer) و نه ذخیره‌سازی (ICartStorage).
///
/// O — OCP: برای اضافه کردن رفتار جدید (مثلاً logging) نیازی به تغییر این کلاس نیست؛
///          می‌توان یک Decorator روی ICartService پیاده کرد.
///
/// L — LSP: این کلاس ICartService را کامل پیاده می‌کند؛
///          هر جا ICartService تزریق شود، این کلاس بدون مشکل جایگزین می‌شود.
///
/// I — ISP: به جای یک Interface بزرگ، مسئولیت‌ها در ICartStorage و ICartSerializer
///          جدا شده‌اند و هر کلاس فقط به Interface مورد نیازش وابسته است.
///
/// D — DIP: این کلاس به ICartStorage و ICartSerializer وابسته است، نه به Session
///          یا JsonSerializer مستقیماً.
/// </summary>
public class SessionCartService : ICartService
{
    private readonly ICartStorage _storage;
    private readonly ICartSerializer _serializer;
    private const string CartKey = "MiniShop_Cart";

    public SessionCartService(ICartStorage storage, ICartSerializer serializer)
    {
        _storage = storage;
        _serializer = serializer;
    }

    public List<CartItem> GetCart()
        => _serializer.Deserialize(_storage.Read(CartKey));

    private void SaveCart(List<CartItem> cart)
        => _storage.Write(CartKey, _serializer.Serialize(cart));

    public void AddItem(int productId, string productName, decimal unitPrice, int quantity = 1)
    {
        var cart = GetCart();
        var existing = cart.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            existing.Quantity += quantity;
        else
            cart.Add(new CartItem { ProductId = productId, ProductName = productName, UnitPrice = unitPrice, Quantity = quantity });
        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return;
        if (quantity <= 0) cart.Remove(item);
        else item.Quantity = quantity;
        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(cart);
    }

    public void Clear() => _storage.Remove(CartKey);

    public decimal GetTotal() => GetCart().Sum(i => i.Subtotal);

    public int GetItemCount() => GetCart().Sum(i => i.Quantity);
}
