namespace Application.Cart;

/// <summary>
/// ISP: مسئولیت خواندن و نوشتن داده‌های خام سبد خرید جدا شده.
/// این Interface مستقل از نحوه ذخیره‌سازی (Session، Redis، DB) است.
/// </summary>
public interface ICartStorage
{
    string? Read(string key);
    void Write(string key, string value);
    void Remove(string key);
}
