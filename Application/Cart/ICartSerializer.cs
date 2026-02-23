namespace Application.Cart;

/// <summary>
/// ISP: مسئولیت سریالایز/دسریالایز سبد خرید جدا شده
/// </summary>
public interface ICartSerializer
{
    string Serialize(List<CartItem> cart);
    List<CartItem> Deserialize(string? json);
}
